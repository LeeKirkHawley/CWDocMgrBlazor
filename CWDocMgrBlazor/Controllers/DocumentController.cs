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
        private readonly UserService _userService;

        public DocumentsController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config, 
            ILogger<DocumentsController> logger, IMapper mapper, DocumentService documentService, UserService userService)
        {
            _db = db;
            _env = env;
            _config = config;
            _logger = logger;
            _mapper = mapper;
            _documentService = documentService;
            _userService = userService;
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

            DocumentModel? document = await _documentService.GetDocumentById(Id.Value);
            if (document == null)
                return NotFound();

            await _documentService.GetDocumentById(Id.Value);

            string? base64FileContent = await _documentService.GetDocumentFileContent(document);
            if(base64FileContent == null)
                return NotFound($"Document file {document.DocumentName} not found.");

            DocumentDetailsVM docDetailsVM = _mapper.Map<DocumentDetailsVM>(document);
            docDetailsVM.FileContent = base64FileContent;

            docDetailsVM.UserName = await _userService.GetUserNameById(document.UserId);

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

            await _documentService.DeleteDocument(document);

            return NoContent();
        }
    }
}
