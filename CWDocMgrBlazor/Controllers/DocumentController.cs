using Microsoft.AspNetCore.Mvc;
using CWDocMgrBlazor.Data;
using SharedLib.Models;
using System.IO;
using CWDocMgrBlazor.Models;
using System.Security.Claims;

namespace CWDocMgrBlazor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public DocumentsController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config)
        {
            _db = db;
            _env = env;
            _config = config;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10_000_000)] // 10 MB, adjust as needed
        public async Task<IActionResult> Upload([FromForm] string OriginalDocumentName, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            string? uploadsFolder = _config["UploadSettings:UploadsFolder"];
            if(uploadsFolder == null)
                return Problem("No configured upload folder.");

            Directory.CreateDirectory(uploadsFolder);

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
                UserId = userId ?? "0"
            };

            _db.Documents.Add(document);
            await _db.SaveChangesAsync();

            return Ok(new { document.Id });
        }
    }
}
