using CWDocMgrBlazor.Controllers;
using Microsoft.IdentityModel.Tokens;

namespace CWDocMgrBlazor.Api.Services
{
    public class PathService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DocumentsController> _logger;
        private readonly IWebHostEnvironment _env;

        public PathService()
        {

        }

        public PathService(IConfiguration config, ILogger<DocumentsController> logger, IWebHostEnvironment env)
        {
            _config = config;
            _logger = logger;
            _env = env;
        }

        public string GetUploadFilePath(string filename)
        {
            string uploadsFolder = GetUploadFolderPath();
            string filePath = Path.Combine(uploadsFolder, filename);
            return filePath;
        }

        public string GetUploadFolderPath()
        {
            string uploadsFolder = Path.Combine(_env.ContentRootPath, _config["UploadsFolder"]);
            if (uploadsFolder == null)
            {
                _logger.LogCritical("No configured upload folder.");
                throw new Exception("No configured upload folder.");
            }
            return uploadsFolder;
        }

        public string GetOCRFilePath(string filename)
        {
            string ocrFileFolder = GetOCRFolderPath();
            string filePath = Path.Combine(ocrFileFolder, filename);
            return filePath;
        }

        public string GetOCRFolderPath()
        {
            _logger.LogInformation($"In GetOCRFolderPath() - _env.ContentRootPath: {_env.ContentRootPath}");

            string configuredOCROutputFolder = _config["OCROutputFolder"];
            if(configuredOCROutputFolder.IsNullOrEmpty())
            {
                _logger.LogCritical("No configured OCR folder.");
                throw new Exception("No configured OCR folder.");
            }

            string uploadsFolder = Path.Combine(_env.ContentRootPath, configuredOCROutputFolder);
            //if (uploadsFolder == null)
            //{
            //}
            return uploadsFolder;
        }


    }
}
