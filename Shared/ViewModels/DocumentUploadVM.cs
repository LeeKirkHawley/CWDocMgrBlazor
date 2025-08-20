namespace SharedLib.Models
{
    public record DocumentUploadVM
    {
        public DocumentUploadVM()
        {
            OCRText = "";
            //User = null;
            DateString = null;
        }

        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required DateTime DocumentDate { get; set; }

        //public virtual UserModel? User { get; set; }
        public string DateString { get; set; }
        public string? OCRText { get; set; }

        public string? FileContent { get; set; }

    }
}
