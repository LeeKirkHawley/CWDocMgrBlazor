using CWDocMgrBlazor.Data;
using CWDocMgrBlazor.Models;
using CWDocMgrBlazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.ViewModels;
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
        private readonly ILogger<DocumentsController> _logger;
        private readonly DocumentService _documentService;
        private readonly UserService _userService;
        private readonly PathService _pathService;

        public DocumentsController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config, 
            ILogger<DocumentsController> logger, DocumentService documentService, UserService userService, PathService pathService)
        {
            _db = db;
            _env = env;
            _config = config;
            _logger = logger;
            _documentService = documentService;
            _userService = userService;
            _pathService = pathService;
        }

        [HttpGet]
        [EnableCors]
        [Authorize]
        public async Task<ActionResult<IEnumerable<DocumentVM>>> GetAll()
        {
            List<DocumentModel> documents = await _documentService.GetAllDocuments();

            List<DocumentVM> docVMs = new List<DocumentVM>();
            foreach (var document in documents)
            {
                DocumentVM documentVM = new DocumentVM {
                    DocumentDate = document.DocumentDate.ToString(),
                    DocumentName = document.DocumentName,
                    UserId = document.UserId,
                    OriginalDocumentName = document.OriginalDocumentName,
                    Id = document.Id,
                };

                docVMs.Add(documentVM);
            }

            return Ok(docVMs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
                return NotFound();

            DocumentModel? document = await _documentService.GetDocumentById(Id.Value);
            if (document == null)
                return NotFound();

            await _documentService.GetDocumentById(Id.Value);

            string uploadFilePath = _pathService.GetUploadFilePath(document.DocumentName);
            string? base64FileContent = await _documentService.GetDocumentFileContent(uploadFilePath);
            if(base64FileContent == null)
                return NotFound($"Document file {document.DocumentName} not found.");

            DocumentDetailsVM docDetailsVM = new DocumentDetailsVM
            {
                UserId = document.UserId,
                DocumentName = document.DocumentName,
                OriginalDocumentName = document.OriginalDocumentName,
                DocumentDate = document.DocumentDate,
                DateString = document.DocumentDate.ToString(),  
                FileContent = base64FileContent,
                UserName = await _userService.GetUserNameById(document.UserId),
                OCRText = document.OCRText
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

            string uploadsFolder = Path.Combine(_env.ContentRootPath, _config["UploadsFolder"]);

            if (uploadsFolder == null)
                return Problem("No configured upload folder.");

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DocumentModel document = await _documentService.UploadFile(dto, uploadsFolder, userId);

            return Ok(new { document.Id });
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _db.Documents.FindAsync(id);
            if (document == null)
                return NotFound();

            string filePath = _pathService.GetUploadFilePath(document.DocumentName);
            await _documentService.DeleteDocument(document, filePath);

            return NoContent();
        }

        [HttpPost("ocrdocument")]
        public async Task<IActionResult> OCRDocument([FromBody] OCRRequestDto dto)
        {
            if (dto.Id <= 0)
                return BadRequest("Invalid document ID.");

            DocumentModel? document = await _db.Documents.FindAsync(dto.Id);
            if (document == null)
                return NotFound("Document not found.");

            string documentFilePath = _pathService.GetUploadFilePath(document.DocumentName);
            if (!System.IO.File.Exists(documentFilePath))
                return NotFound("Document file not found.");

            string ocrFileFolder = _pathService.GetOCRFolderPath();
            string ocrText = await _documentService.OCRDocument(document, ocrFileFolder, documentFilePath, ocrFileFolder);

            document.OCRText = ocrText;
            await _db.SaveChangesAsync();

            return Ok(new { ocrText });
        }

    }
}
