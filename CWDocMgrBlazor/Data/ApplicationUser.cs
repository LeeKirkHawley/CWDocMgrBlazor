using CWDocMgrBlazor.Models;
using Microsoft.AspNetCore.Identity;

namespace CWDocMgrBlazor.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public int? OrganizationId { get; set; }

    // Navigation property
    public virtual Organization? Organization { get; set; }

}

