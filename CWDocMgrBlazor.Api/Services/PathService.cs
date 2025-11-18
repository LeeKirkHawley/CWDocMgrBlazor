using Microsoft.IdentityModel.Tokens;

namespace CWDocMgrBlazor.Api.Services
{
    public class PathService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<PathService> _logger;
        private readonly IWebHostEnvironment _env;

        public PathService(IConfiguration config, ILogger<PathService> logger, IWebHostEnvironment env)
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
            string configuredFolder = _config["UploadsFolder"];
            if (string.IsNullOrEmpty(configuredFolder))
            {
                _logger.LogCritical("No configured upload folder.");
                throw new Exception("No configured upload folder.");
            }

            // Use relative path for production
            string uploadsFolder = Path.IsPathRooted(configuredFolder)
                ? configuredFolder
                : Path.Combine(_env.ContentRootPath, configuredFolder);

            // Ensure directory exists
            Directory.CreateDirectory(uploadsFolder);
            
            _logger.LogInformation($"Upload folder path: {uploadsFolder}");
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
            if (string.IsNullOrEmpty(configuredOCROutputFolder))
            {
                _logger.LogCritical("No configured OCR folder.");
                throw new Exception("No configured OCR folder.");
            }

            // Use relative path for production
            string ocrFolder = Path.IsPathRooted(configuredOCROutputFolder)
                ? configuredOCROutputFolder
                : Path.Combine(_env.ContentRootPath, configuredOCROutputFolder);

            // Ensure directory exists
            Directory.CreateDirectory(ocrFolder);
            
            _logger.LogInformation($"OCR folder path: {ocrFolder}");
            return ocrFolder;
        }
    }
}
