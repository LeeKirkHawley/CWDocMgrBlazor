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


        public DocumentService(ILogger<DocumentService> logger, IConfiguration config, ApplicationDbContext db, OCRService ocrService) 
        {
            _logger = logger;
            _config = config;
            _db = db;
            _ocrService = ocrService;
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

        public async Task<string?> GetDocumentFileContent(DocumentModel document) 
        {
            byte[] fileBytes = [];
            string ? base64FileContent = null;
            string documentFilePath = _config["UploadsFolder"] + "/" + document.DocumentName;

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

        public async Task DeleteDocument(DocumentModel document)
        {
            var uploadsFolder = _config["UploadsFolder"];
            if (!string.IsNullOrEmpty(uploadsFolder))
            {
                var filePath = Path.Combine(uploadsFolder, document.DocumentName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _db.Documents.Remove(document);
            await _db.SaveChangesAsync();
        }

        public async Task<string> OCRDocument(DocumentModel document)
        {
            try
            {
                _logger.LogInformation($"Starting OCR for document: {document.DocumentName}");

                // Get the OCR output folder
                string ocrOutputFolder = _config["OCROutputFolder"];
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
                    string documentFullPath = Path.Combine(_config["UploadsFolder"], document.DocumentName);
                    await _ocrService.OCRPDFFile(documentFullPath, outputBasePath, "eng");
                }
                else
                {
                    // For non-PDF files, just pass the document name
                    errorMsg = _ocrService.OCRImageFile(document.DocumentName, outputBasePath, "eng");
                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        _logger.LogWarning($"OCR returned error: {errorMsg}");
                        return $"OCR Error: {errorMsg}";
                    }
                }

                // Get the OCR text from the generated file
                string ocrText = _ocrService.GetOcrFileText(document.DocumentName);

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

        //public async Task<string> OCRDocument(DocumentModel document)
        //{
        //    string ocrFolder = _config["OCROutputFolder"];
        //    _ocrService.OCRImageFile(document.DocumentName, ocrFolder, "eng");

        //    return "Not implemented yet";
        //}

    }
}
