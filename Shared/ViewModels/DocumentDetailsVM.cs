namespace SharedLib.ViewModels
{
    public class DocumentDetailsVM
    {
        public DocumentDetailsVM()
        {
            OCRText = "";
            DateString = null;
        }

        public int Id { get; set; }
        public required string UserId { get; set; }
        public string UserName { get; set; } = "Unknown User";
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required DateTime DocumentDate { get; set; }

        public required string DateString { get; set; }
        public string? OCRText { get; set; }

        public string? FileContent { get; set; }

    }
}
