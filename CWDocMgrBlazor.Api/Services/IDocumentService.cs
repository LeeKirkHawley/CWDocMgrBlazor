using CWDocMgrBlazor.Api.Models;
using SharedLib.DTOs;

namespace CWDocMgrBlazor.Api.Services
{
    public interface IDocumentService
    {
        Task DeleteDocument(DocumentModel document, string filePath);
        Task<List<DocumentModel>> GetAllDocuments();
        Task<DocumentModel?> GetDocumentById(int id);
        Task<string?> GetDocumentFileContent(string documentFilePath);
        Task<string> OCRDocument(DocumentModel document, string ocrOutputFolder, string uploadFilePath, string rootPath);
        Task<DocumentModel> UploadFile(DocumentUploadDto dto, string uploadsFolder, string? userId);
    }
}