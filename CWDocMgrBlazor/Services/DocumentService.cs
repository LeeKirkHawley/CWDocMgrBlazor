using CWDocMgrBlazor.Data;
using CWDocMgrBlazor.Models;
using DocMgrLib.Services;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;

namespace CWDocMgrBlazor.Services
{

    public class DocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        public readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;
        private readonly OCRService _ocrService;
        private readonly PathService _pathService;

        public DocumentService(ILogger<DocumentService> logger, IConfiguration config, ApplicationDbContext db, 
            OCRService ocrService, PathService pathService) 
        {
            _logger = logger;
            _config = config;
            _db = db;
            _ocrService = ocrService;
            _pathService = pathService;
        }

        public async Task<List<DocumentModel>> GetAllDocuments() 
        {
            _logger.LogDebug("Retrieving all documents from the database.");
            List<DocumentModel> docs = await _db.Documents.ToListAsync();
            _logger.LogDebug($"Got {docs.Count} documents.");

            return docs;
        }

        public async Task<DocumentModel?> GetDocumentById(int id)
        {
            _logger.LogDebug($"Retrieving document with ID {id} from the database.");
            DocumentModel? document = await _db.Documents.FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
            {
                _logger.LogWarning($"Document with ID {id} not found.");
                return null;
            }

            return document;
        }

        public async Task<string?> GetDocumentFileContent(string documentFilePath) 
        {
            byte[] fileBytes = [];
            string ? base64FileContent = null;

            if (System.IO.File.Exists(documentFilePath))
            {
                fileBytes = await System.IO.File.ReadAllBytesAsync(documentFilePath);
                base64FileContent = $"data:image/jpeg;base64,{Convert.ToBase64String(fileBytes)}";
            }

            return base64FileContent;
        }

        public async Task<DocumentModel> UploadFile(DocumentUploadDto dto, string uploadsFolder, string? userId)
        {
            Directory.CreateDirectory(uploadsFolder);

            string fileExtension = Path.GetExtension(dto.OriginalDocumentName);
            string fileName = Guid.NewGuid().ToString();
            string newFileName = $"{fileName}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, newFileName);

            _logger.LogInformation($"Uploading file to: {filePath}");

            using (var stream = System.IO.File.Create(filePath))
            {
                await dto.File.CopyToAsync(stream);
            }

            var document = new DocumentModel
            {
                DocumentName = newFileName,
                OriginalDocumentName = dto.OriginalDocumentName,
                DocumentDate = DateTime.UtcNow,
                UserId = userId ?? "0"
            };

            _db.Documents.Add(document);
            await _db.SaveChangesAsync();
            return document;
        }

        public async Task DeleteDocument(DocumentModel document, string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _db.Documents.Remove(document);
            await _db.SaveChangesAsync();
        }

        public async Task<string> OCRDocument(DocumentModel document, string ocrOutputFolder, string uploadFilePath, string rootPath)
        {
            _logger.LogInformation($"In OCRDocument(): DocumentName is {document.DocumentName} - ocrOutputFolder is {ocrOutputFolder} - uploadFilePath is {uploadFilePath} - rootPath is {rootPath}");

            try
            {
                // Get the OCR output folder
                if (string.IsNullOrEmpty(ocrOutputFolder))
                {
                    _logger.LogError("OCROutputFolder configuration is missing");
                    return "Error: OCR output folder not configured";
                }

                // Ensure the OCR output directory exists
                Directory.CreateDirectory(ocrOutputFolder);

                // Get the file name without extension
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(document.DocumentName);

                // Create output base path (without file extension)
                string outputBasePath = Path.Combine(ocrOutputFolder, fileNameWithoutExtension);

                // Process the document based on file type
                string extension = Path.GetExtension(document.DocumentName).ToLowerInvariant();
                string errorMsg = "";

                if (extension == ".pdf")
                {
                    // Full path to the document
                    string documentFullPath = _pathService.GetUploadFilePath(document.DocumentName);
                    await _ocrService.OCRPDFFile(documentFullPath, outputBasePath, "eng", _pathService.GetUploadFolderPath());
                }
                else
                {
                    // For non-PDF files, just pass the document name

                    errorMsg = _ocrService.OCRImageFile(document.DocumentName, outputBasePath, "eng", uploadFilePath);
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        _logger.LogWarning($"OCR returned error: {errorMsg}");
                        return $"OCR Error: {errorMsg}";
                    }
                }

                // Get the OCR text from the generated file
                string ocrText = _ocrService.GetOcrFileText(document, rootPath);
                _ocrService.OCRCleanup(document, rootPath);


                if (string.IsNullOrEmpty(ocrText))
                {
                    ocrText = "No text was extracted from the document.";
                }

                _logger.LogInformation($"OCR completed for document: {document.DocumentName}");
                return ocrText;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error performing OCR on document {document.DocumentName}");
                return $"OCR failed: {ex.Message}";
            }
        }
    }
}
