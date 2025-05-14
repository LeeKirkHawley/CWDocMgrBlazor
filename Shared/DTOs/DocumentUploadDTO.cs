using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.DTOs
{
    public class DocumentUploadDto
    {
        public string DocumentName { get; set; } = string.Empty;
        public string OriginalDocumentName { get; set; } = string.Empty;
        public DateTime DocumentDate { get; set; }
        public IFormFile File { get; set; } = default!;
    }
}
