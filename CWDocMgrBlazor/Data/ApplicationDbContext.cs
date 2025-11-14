using CWDocMgrBlazor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace CWDocMgrBlazor.Data;


public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<DocumentModel> Documents { get; set; }
    public DbSet<Organization> Organizations { get; set; }

    // Configure Organization-User relationship
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Organization-User relationship (Users belonging to Organization)
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(u => u.OrganizationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Organization-Administrator relationship
        builder.Entity<Organization>()
            .HasOne(o => o.Administrator)
            .WithMany()
            .HasForeignKey(o => o.AdministratorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting admin if they have organizations
    }
}
