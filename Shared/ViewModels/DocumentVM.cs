using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.ViewModels
{
    public class DocumentVM
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string DocumentName { get; set; }
        public required string OriginalDocumentName { get; set; }
        public required string DocumentDate { get; set; }
    }
}
