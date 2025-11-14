using CWDocMgrBlazor.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CWDocMgrBlazor.Models
{
    public record Organization
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string OrgFolderName { get; set; }
        public required string AdministratorId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        [ForeignKey("AdministratorId")]
        public ApplicationUser? Administrator { get; set; }

        // Navigation property for users belonging to this organization
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
