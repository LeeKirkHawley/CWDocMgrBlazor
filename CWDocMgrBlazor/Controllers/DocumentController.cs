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
using SharedLib.ViewModels;
using AutoMapper;
using CWDocMgrBlazor.Services;

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
        private readonly IMapper _mapper;
        private readonly DocumentService _documentService;

        public DocumentsController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config, 
            ILogger<DocumentsController> logger, IMapper mapper, DocumentService documentService)
        {
            _db = db;
            _env = env;
            _config = config;
            _logger = logger;
            _mapper = mapper;
            _documentService = documentService;
        }

        [HttpGet]
        [EnableCors]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DocumentVM>>> GetAll()
        {
            List<DocumentModel> documents = await _documentService.GetAllDocuments();
            List<DocumentVM> docVMs = _mapper.Map<List<DocumentVM>>(documents);

            return Ok(docVMs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
                return NotFound();

            var document = await _documentService.GetDocumentById(Id.Value);
            if (document == null)
                return NotFound();

            await _documentService.GetDocumentById(Id.Value);

            string? base64FileContent = await _documentService.GetDocumentFileContent(document);
            if(base64FileContent == null)
                return NotFound($"Document file {document.DocumentName} not found.");

            DocumentUploadVM docDetailsVM = _mapper.Map<DocumentUploadVM>(document);
            docDetailsVM.FileContent = base64FileContent;

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
                DocumentDate = DateTime.UtcNow,
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
