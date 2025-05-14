namespace CWDocMgrBlazor.Models
{
    public record DocumentModel
    {
        public int Id { get; set; }
        //[ForeignKey("UserId")]
        //public required string UserId { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required DateTime DocumentDate { get; set; }

    }
}
