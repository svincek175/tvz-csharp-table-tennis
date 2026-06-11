using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Web.Infrastructure.Identity;

namespace TableTennisTracker.Web.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[] { "Admin", "Editor", "User" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role) { Id = Guid.NewGuid() });
            }
        }
    }

    public static async Task SeedAdminUserAsync(UserManager<AppUser> userManager)
    {
        var adminExists = await userManager.FindByEmailAsync("admin@tabletennistrack.local");
        if (adminExists != null)
        {
            return;
        }

        var adminUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Email = "admin@tabletennistrack.local",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User",
            CreatedUtc = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    public static async Task SeedAsync(TableTennisTrackerDbContext dbContext)
    {
        if (await dbContext.Venues.AnyAsync())
        {
            return;
        }

        var (venues, tournaments, players, _, _, _, _) = DataSeeder.SeedData();

        await dbContext.Venues.AddRangeAsync(venues);
        await dbContext.Players.AddRangeAsync(players);
        await dbContext.Tournaments.AddRangeAsync(tournaments);

        await dbContext.SaveChangesAsync();
    }
}
