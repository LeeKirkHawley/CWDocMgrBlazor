using Microsoft.AspNetCore.Mvc;
using CWDocMgrBlazor.Data;
using SharedLib.Models;
using System.IO;
using CWDocMgrBlazor.Models;

namespace CWDocMgrBlazor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public DocumentsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // 10 MB, adjust as needed
        public async Task<IActionResult> Upload([FromForm] string OriginalDocumentName, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string uploadsFolder = "C:\\Temp\\UploadedDocuments";
            Directory.CreateDirectory(uploadsFolder);

            string fileExtension = Path.GetExtension(OriginalDocumentName);
            string fileName = Guid.NewGuid().ToString();
            string newFileName = $"{fileName}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, newFileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            var document = new DocumentModel
            {
                DocumentName = filePath,
                OriginalDocumentName = OriginalDocumentName,
                DocumentDate = DateTime.Now,
                UserId = "0" // Replace with actual user ID if needed
            };

            _db.Documents.Add(document);
            await _db.SaveChangesAsync();

            return Ok(new { document.Id });
        }
    }
}
