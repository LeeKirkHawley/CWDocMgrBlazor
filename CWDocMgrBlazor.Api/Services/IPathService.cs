namespace CWDocMgrBlazor.Api.Services
{
    public interface IPathService
    {
        string GetOCRFilePath(string filename);
        string GetOCRFolderPath();
        string GetUploadFilePath(string filename);
        string GetUploadFolderPath();
    }
}