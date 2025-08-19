using Microsoft.AspNetCore.Mvc;
using CWDocMgrBlazor.Data;
using SharedLib.Models;
using System.IO;
using CWDocMgrBlazor.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace CWDocMgrBlazor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config, ILogger<DocumentsController> logger)
        {
            _db = db;
            _env = env;
            _config = config;
            _logger = logger;
        }

        [HttpGet]
        [EnableCors]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DocumentModel>>> GetAll()
        {
            _logger.LogDebug("Retrieving all documents from the database.");
            var docs = await _db.Documents.ToListAsync();
            _logger.LogDebug("Got em.");
            return Ok(docs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            DocumentModel? document = await _db.Documents.FirstOrDefaultAsync(m => m.Id == id);
            if (document == null)
                return NotFound();

            string? base64FileContent = null;
            string documentFilePath = _config["UploadsFolder"] + "/" + document.DocumentName;
            if (System.IO.File.Exists(documentFilePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(documentFilePath);
                base64FileContent = $"data:image/jpeg;base64,{Convert.ToBase64String(fileBytes)}";
            }

//            string ocrText = _ocrService.GetOcrFileText(documentFilePath);

            DocumentVM docDetailsVM = new DocumentVM
            {
                Id = document.Id,
                UserId = document.UserId,
                DocumentName = document.DocumentName,
                DocumentDate = document.DocumentDate,
                OriginalDocumentName = document.OriginalDocumentName,
                FileContent = base64FileContent,
                OCRText = ""
            };

            return Ok(docDetailsVM);
        }


        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // 10 MB, adjust as needed
        public async Task<IActionResult> Upload([FromForm] DocumentUploadDto dto)
        {
            _logger.LogDebug("Uploading file(s)");

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("No file uploaded.");

            string? uploadsFolder = _config["UploadsFolder"];
            if(uploadsFolder == null)
                return Problem("No configured upload folder.");

            Directory.CreateDirectory(uploadsFolder);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                DocumentDate = DateTime.Now,
                UserId = userId ?? "0"
            };

            _db.Documents.Add(document);
            await _db.SaveChangesAsync();

            return Ok(new { document.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _db.Documents.FindAsync(id);
            if (document == null)
                return NotFound();

            // Optionally delete the file from disk
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

            return NoContent();
        }

    }
}
