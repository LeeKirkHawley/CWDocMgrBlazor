using CWDocMgrBlazor.Data;
using CWDocMgrBlazor.Models;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using System.Security.Claims;

namespace CWDocMgrBlazor.Services
{

    public class DocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        public IConfiguration _config;
        private readonly ApplicationDbContext _db;


        public DocumentService(ILogger<DocumentService> logger, IConfiguration config, ApplicationDbContext db) 
        {
            _logger = logger;
            _config = config;
            _db = db;
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

    }
}
