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
        public string OriginalDocumentName { get; set; } = string.Empty;
        public IFormFile File { get; set; } = default!;
    }
}
