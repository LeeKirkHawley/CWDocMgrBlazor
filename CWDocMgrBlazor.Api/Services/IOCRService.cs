using CWDocMgrBlazor.Api.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CWDocMgrBlazor.Api.Services
{
    public interface IOCRService
    {
        void Cleanup(string imageFilePath, string textFilePath);
        string GetOCRFilePath(DocumentModel documentModel, string uploadsFolder);
        string GetOcrFileText(DocumentModel documentModel, string rootPath);
        void ImmediateCleanup(string imageFilePath, string imageFileExtension, string textFilePath);
        bool IsOCRable(DocumentModel document);
        void OCRCleanup(DocumentModel documentModel, string rootPath);
        Task<string> OCRImageFile(string imageName, string language, string imagePath);
        Task<string> OCRPDFDocument(DocumentModel document, string language, string UploadsFolder);
        List<SelectListItem> SetupLanguages();
    }
}