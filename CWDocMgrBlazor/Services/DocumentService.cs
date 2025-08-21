using Microsoft.EntityFrameworkCore;
using CWDocMgrBlazor.Data;
using CWDocMgrBlazor.Models;

namespace CWDocMgrBlazor.Services
{

    public class DocumentService
    {
        private readonly ILogger<DocumentService> _logger;
        private readonly ApplicationDbContext _db;


        public DocumentService(ILogger<DocumentService> logger, ApplicationDbContext db) 
        {
            _logger = logger;
            _db = db;
        }

        public async Task<List<DocumentModel>> GetAllDocuments() 
        {
            _logger.LogDebug("Retrieving all documents from the database.");
            List<DocumentModel> docs = await _db.Documents.ToListAsync();
            _logger.LogDebug($"Got {docs.Count} documents.");

            return docs;
        }
    }
}
