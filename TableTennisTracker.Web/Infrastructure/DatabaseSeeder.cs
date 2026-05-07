using Microsoft.EntityFrameworkCore;

namespace TableTennisTracker.Web.Infrastructure;

public static class DatabaseSeeder
{
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
