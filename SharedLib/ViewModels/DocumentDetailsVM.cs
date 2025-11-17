namespace SharedLib.ViewModels
{
    public class DocumentDetailsVM
    {
        public required string UserId { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public DateTime DocumentDate { get; set; }
        public required string DateString { get; set; }
        public required string FileContent { get; set; }
        public required string UserName { get; set; }
        public string? OCRText { get; set; }
    }
}