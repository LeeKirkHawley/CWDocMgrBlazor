using CWDocMgrBlazor.Api.Services;
using CWDocMgrBlazor.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using CWDocMgrBlazor.Api.Data;
using SharedLib.ViewModels;
using CWDocMgrBlazor.Api.Models;

namespace CWDocMgrBlazor.Api.Controllers
{
    public class DocumentController : ControllerBase
    {
        private readonly Data.ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly ILogger<DocumentController> _logger;
        private readonly DocumentService _documentService;
        private readonly UserService _userService;
        private readonly PathService _pathService;

        public DocumentController(ApplicationDbContext db, IWebHostEnvironment env, IConfiguration config,
            ILogger<DocumentController> logger, DocumentService documentService, UserService userService, PathService pathService)
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
            _logger.LogDebug("In GetAll endpoint");
            _logger.LogInformation("In GetAll endpoint");

            List<DocumentModel> documents = await _documentService.GetAllDocuments();

            List<DocumentVM> docVMs = new List<DocumentVM>();
            foreach (var document in documents)
            {
                DocumentVM documentVM = new DocumentVM
                {
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
    }
}
