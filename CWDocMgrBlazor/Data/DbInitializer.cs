using CWDocMgrBlazor.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CWDocMgrBlazor.Data;

public static class DbInitializer
{
    public static async Task Initialize(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created (or apply migrations)
            await context.Database.MigrateAsync();

            // Seed admin user
            await SeedAdminUser(userManager, roleManager, configuration);

            await SeedAdditionalRoles(roleManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    private static async Task SeedAdditionalRoles(RoleManager<IdentityRole> roleManager)
    {
        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Create User role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("User"))
        {
            await roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Create User role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("ReadOnly"))
        {
            await roleManager.CreateAsync(new IdentityRole("ReadOnly"));
        }

        // Create User role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("OrgManager"))
        {
            await roleManager.CreateAsync(new IdentityRole("OrgManager"));
        }


    }


    private static async Task SeedAdminUser(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        // Create Admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Get admin credentials from configuration
        var adminEmail = configuration["AdminUser:Email"] ?? "admin@example.com";
        var adminPassword = configuration["AdminUser:Password"] ?? "P@ssword1!";

        // Check if admin user exists
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            // Create the admin user
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true // Skip email confirmation
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Add to Admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}