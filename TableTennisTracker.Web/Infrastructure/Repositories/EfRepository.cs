using Microsoft.EntityFrameworkCore;
using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Infrastructure.Repositories;

public abstract class EfRepositoryBase
{
    protected readonly TableTennisTrackerDbContext DbContext;

    protected EfRepositoryBase(TableTennisTrackerDbContext dbContext)
    {
        DbContext = dbContext;
    }

    protected static string NormalizeFilter(string? filter)
    {
        return string.IsNullOrWhiteSpace(filter) ? "all" : filter.Trim().ToLowerInvariant();
    }

    protected static IQueryable<Tournament> ApplyTournamentFilter(IQueryable<Tournament> query, string lowerQuery, string? filter)
    {
        return NormalizeFilter(filter) switch
        {
            "name" => query.Where(t => t.Name.ToLower().Contains(lowerQuery)),
            "season" => query.Where(t => t.SeasonLabel.ToLower().Contains(lowerQuery)),
            "organizer" => query.Where(t => t.OrganizerName.ToLower().Contains(lowerQuery)),
            _ => query.Where(t => t.Name.ToLower().Contains(lowerQuery) ||
                                   t.SeasonLabel.ToLower().Contains(lowerQuery) ||
                                   t.OrganizerName.ToLower().Contains(lowerQuery))
        };
    }

    protected static IQueryable<Venue> ApplyVenueFilter(IQueryable<Venue> query, string lowerQuery, string? filter)
    {
        return NormalizeFilter(filter) switch
        {
            "name" => query.Where(v => v.Name.ToLower().Contains(lowerQuery)),
            "city" => query.Where(v => v.City.ToLower().Contains(lowerQuery)),
            "country" => query.Where(v => v.CountryCode.ToLower().Contains(lowerQuery)),
            _ => query.Where(v => v.Name.ToLower().Contains(lowerQuery) ||
                                  v.City.ToLower().Contains(lowerQuery) ||
                                  v.CountryCode.ToLower().Contains(lowerQuery))
        };
    }

    protected static IQueryable<Player> ApplyPlayerFilter(IQueryable<Player> query, string lowerQuery, string? filter)
    {
        return NormalizeFilter(filter) switch
        {
            "name" => query.Where(p => p.FirstName.ToLower().Contains(lowerQuery) ||
                                        p.LastName.ToLower().Contains(lowerQuery)),
            "country" => query.Where(p => p.CountryCode.ToLower().Contains(lowerQuery)),
            "status" => query.Where(p => (p.IsActive ? "active" : "inactive").Contains(lowerQuery)),
            _ => query.Where(p => p.FirstName.ToLower().Contains(lowerQuery) ||
                                   p.LastName.ToLower().Contains(lowerQuery) ||
                                   p.CountryCode.ToLower().Contains(lowerQuery))
        };
    }

    protected static IQueryable<Match> ApplyMatchFilter(IQueryable<Match> query, string lowerQuery, string? filter)
    {
        return NormalizeFilter(filter) switch
        {
            "tournament" => query.Where(m => m.Tournament.Name.ToLower().Contains(lowerQuery)),
            "round" => query.Where(m => m.RoundNumber.ToString().Contains(lowerQuery)),
            "status" => query.Where(m => m.Status.ToString().ToLower().Contains(lowerQuery)),
            _ => query.Where(m => m.Tournament.Name.ToLower().Contains(lowerQuery) ||
                                  m.RoundNumber.ToString().Contains(lowerQuery) ||
                                  m.Status.ToString().ToLower().Contains(lowerQuery))
        };
    }

    protected static IQueryable<Registration> ApplyRegistrationFilter(IQueryable<Registration> query, string lowerQuery, string? filter)
    {
        return NormalizeFilter(filter) switch
        {
            "player" => query.Where(r => r.Player.FirstName.ToLower().Contains(lowerQuery) ||
                                          r.Player.LastName.ToLower().Contains(lowerQuery)),
            "tournament" => query.Where(r => r.Tournament.Name.ToLower().Contains(lowerQuery)),
            "checkedin" => query.Where(r => (r.IsCheckedIn ? "checked in" : "not checked in").Contains(lowerQuery)),
            _ => query.Where(r => r.Player.FirstName.ToLower().Contains(lowerQuery) ||
                                   r.Player.LastName.ToLower().Contains(lowerQuery) ||
                                   r.Tournament.Name.ToLower().Contains(lowerQuery))
        };
    }

    protected static IQueryable<MatchParticipant> ApplyMatchParticipantFilter(IQueryable<MatchParticipant> query, string lowerQuery, string? filter)
    {
        return NormalizeFilter(filter) switch
        {
            "player" => query.Where(mp => mp.Player.FirstName.ToLower().Contains(lowerQuery) ||
                                           mp.Player.LastName.ToLower().Contains(lowerQuery)),
            "match" => query.Where(mp => mp.Match.Tournament.Name.ToLower().Contains(lowerQuery) ||
                                          mp.Match.RoundNumber.ToString().Contains(lowerQuery)),
            "slot" => query.Where(mp => mp.Slot.ToString().Contains(lowerQuery)),
            _ => query.Where(mp => mp.Player.FirstName.ToLower().Contains(lowerQuery) ||
                                   mp.Player.LastName.ToLower().Contains(lowerQuery) ||
                                   mp.Match.Tournament.Name.ToLower().Contains(lowerQuery))
        };
    }

    protected static IQueryable<MatchSetResult> ApplyMatchSetResultFilter(IQueryable<MatchSetResult> query, string lowerQuery, string? filter)
    {
        return NormalizeFilter(filter) switch
        {
            "tournament" => query.Where(sr => sr.Match.Tournament.Name.ToLower().Contains(lowerQuery)),
            "set" => query.Where(sr => sr.SetNumber.ToString().Contains(lowerQuery)),
            "score" => query.Where(sr => sr.PlayerOnePoints.ToString().Contains(lowerQuery) ||
                                          sr.PlayerTwoPoints.ToString().Contains(lowerQuery)),
            _ => query.Where(sr => sr.Match.Tournament.Name.ToLower().Contains(lowerQuery) ||
                                   sr.SetNumber.ToString().Contains(lowerQuery))
        };
    }
}

public class EfTournamentRepository : EfRepositoryBase, ITournamentRepository
{
    public EfTournamentRepository(TableTennisTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Tournament>> GetAllAsync()
    {
        return await DbContext.Tournaments
            .AsNoTracking()
            .Include(t => t.Venue)
            .Include(t => t.Registrations)
                .ThenInclude(r => r.Player)
            .Include(t => t.Matches)
                .ThenInclude(m => m.Participants)
                    .ThenInclude(mp => mp.Player)
            .Include(t => t.Matches)
                .ThenInclude(m => m.SetResults)
            .Include(t => t.Matches)
                .ThenInclude(m => m.WinnerPlayer)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Tournament>> SearchAsync(string query, string? filter = null)
    {
        var lowerQuery = query.ToLower();
        var tournaments = DbContext.Tournaments
            .AsNoTracking()
            .Include(t => t.Venue)
            .AsSplitQuery()
            .AsQueryable();

        tournaments = ApplyTournamentFilter(tournaments, lowerQuery, filter);
        return await tournaments.ToListAsync();
    }

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await DbContext.Tournaments
            .AsNoTracking()
            .Include(t => t.Venue)
            .Include(t => t.Registrations)
                .ThenInclude(r => r.Player)
            .Include(t => t.Matches)
                .ThenInclude(m => m.Participants)
                    .ThenInclude(mp => mp.Player)
            .Include(t => t.Matches)
                .ThenInclude(m => m.SetResults)
            .Include(t => t.Matches)
                .ThenInclude(m => m.WinnerPlayer)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Guid> CreateAsync(Tournament tournament)
    {
        tournament.Id = Guid.NewGuid();
        DbContext.Tournaments.Add(tournament);
        await DbContext.SaveChangesAsync();
        return tournament.Id;
    }

    public async Task UpdateAsync(Tournament tournament)
    {
        DbContext.Tournaments.Update(tournament);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tournament = await DbContext.Tournaments.FindAsync(id);
        if (tournament != null)
        {
            DbContext.Tournaments.Remove(tournament);
            await DbContext.SaveChangesAsync();
        }
    }
}

public class EfVenueRepository : EfRepositoryBase, IVenueRepository
{
    public EfVenueRepository(TableTennisTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Venue>> GetAllAsync()
    {
        return await DbContext.Venues
            .AsNoTracking()
            .Include(v => v.Tournaments)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Venue>> SearchAsync(string query, string? filter = null)
    {
        var lowerQuery = query.ToLower();
        var venues = DbContext.Venues
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        venues = ApplyVenueFilter(venues, lowerQuery, filter);
        return await venues.ToListAsync();
    }

    public async Task<Venue?> GetByIdAsync(Guid id)
    {
        return await DbContext.Venues
            .AsNoTracking()
            .Include(v => v.Tournaments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Guid> CreateAsync(Venue venue)
    {
        venue.Id = Guid.NewGuid();
        DbContext.Venues.Add(venue);
        await DbContext.SaveChangesAsync();
        return venue.Id;
    }

    public async Task UpdateAsync(Venue venue)
    {
        DbContext.Venues.Update(venue);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var venue = await DbContext.Venues.FindAsync(id);
        if (venue != null)
        {
            DbContext.Venues.Remove(venue);
            await DbContext.SaveChangesAsync();
        }
    }
}

public class EfPlayerRepository : EfRepositoryBase, IPlayerRepository
{
    public EfPlayerRepository(TableTennisTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Player>> GetAllAsync()
    {
        return await DbContext.Players
            .AsNoTracking()
            .Include(p => p.Registrations)
                .ThenInclude(r => r.Tournament)
            .Include(p => p.MatchParticipations)
                .ThenInclude(mp => mp.Match)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Player>> SearchAsync(string query, string? filter = null)
    {
        var lowerQuery = query.ToLower();
        var players = DbContext.Players
            .AsNoTracking()
            .AsSplitQuery()
            .AsQueryable();

        players = ApplyPlayerFilter(players, lowerQuery, filter);
        return await players.ToListAsync();
    }

    public async Task<Player?> GetByIdAsync(Guid id)
    {
        return await DbContext.Players
            .AsNoTracking()
            .Include(p => p.Registrations)
                .ThenInclude(r => r.Tournament)
            .Include(p => p.MatchParticipations)
                .ThenInclude(mp => mp.Match)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Guid> CreateAsync(Player player)
    {
        player.Id = Guid.NewGuid();
        player.CreatedUtc = DateTime.UtcNow;
        DbContext.Players.Add(player);
        await DbContext.SaveChangesAsync();
        return player.Id;
    }

    public async Task UpdateAsync(Player player)
    {
        DbContext.Players.Update(player);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var player = await DbContext.Players.FindAsync(id);
        if (player != null)
        {
            DbContext.Players.Remove(player);
            await DbContext.SaveChangesAsync();
        }
    }
}

public class EfMatchRepository : EfRepositoryBase, IMatchRepository
{
    public EfMatchRepository(TableTennisTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Match>> GetAllAsync()
    {
        return await DbContext.Matches
            .AsNoTracking()
            .Include(m => m.Tournament)
                .ThenInclude(t => t.Venue)
            .Include(m => m.WinnerPlayer)
            .Include(m => m.Participants)
                .ThenInclude(mp => mp.Player)
            .Include(m => m.SetResults)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Match>> SearchAsync(string query, string? filter = null)
    {
        var lowerQuery = query.ToLower();
        var matches = DbContext.Matches
            .AsNoTracking()
            .Include(m => m.Tournament)
                .ThenInclude(t => t.Venue)
            .Include(m => m.WinnerPlayer)
            .Include(m => m.Participants)
                .ThenInclude(mp => mp.Player)
            .Include(m => m.SetResults)
            .AsSplitQuery()
            .AsQueryable();

        matches = ApplyMatchFilter(matches, lowerQuery, filter);
        return await matches.ToListAsync();
    }

    public async Task<Match?> GetByIdAsync(Guid id)
    {
        return await DbContext.Matches
            .AsNoTracking()
            .Include(m => m.Tournament)
                .ThenInclude(t => t.Venue)
            .Include(m => m.WinnerPlayer)
            .Include(m => m.Participants)
                .ThenInclude(mp => mp.Player)
            .Include(m => m.SetResults)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid tournamentId)
    {
        return await DbContext.Matches
            .AsNoTracking()
            .Include(m => m.Tournament)
                .ThenInclude(t => t.Venue)
            .Include(m => m.WinnerPlayer)
            .Include(m => m.Participants)
                .ThenInclude(mp => mp.Player)
            .Include(m => m.SetResults)
            .AsSplitQuery()
            .Where(m => m.TournamentId == tournamentId)
            .ToListAsync();
    }

    public async Task<Guid> CreateAsync(Match match)
    {
        match.Id = Guid.NewGuid();
        DbContext.Matches.Add(match);
        await DbContext.SaveChangesAsync();
        return match.Id;
    }

    public async Task UpdateAsync(Match match)
    {
        DbContext.Matches.Update(match);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var match = await DbContext.Matches.FindAsync(id);
        if (match != null)
        {
            DbContext.Matches.Remove(match);
            await DbContext.SaveChangesAsync();
        }
    }
}

public class EfRegistrationRepository : EfRepositoryBase, IRegistrationRepository
{
    public EfRegistrationRepository(TableTennisTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Registration>> GetAllAsync()
    {
        return await DbContext.Registrations
            .AsNoTracking()
            .Include(r => r.Player)
            .Include(r => r.Tournament)
                .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<Registration>> SearchAsync(string query, string? filter = null)
    {
        var lowerQuery = query.ToLower();
        var registrations = DbContext.Registrations
            .AsNoTracking()
            .Include(r => r.Player)
            .Include(r => r.Tournament)
                .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .AsQueryable();

        registrations = ApplyRegistrationFilter(registrations, lowerQuery, filter);
        return await registrations.ToListAsync();
    }

    public async Task<Registration?> GetByIdAsync(Guid id)
    {
        return await DbContext.Registrations
            .AsNoTracking()
            .Include(r => r.Player)
            .Include(r => r.Tournament)
                .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Registration>> GetByTournamentIdAsync(Guid tournamentId)
    {
        return await DbContext.Registrations
            .AsNoTracking()
            .Include(r => r.Player)
            .Include(r => r.Tournament)
                .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .Where(r => r.TournamentId == tournamentId)
            .ToListAsync();
    }

    public async Task<Guid> CreateAsync(Registration registration)
    {
        registration.Id = Guid.NewGuid();
        registration.RegisteredUtc = DateTime.UtcNow;
        DbContext.Registrations.Add(registration);
        await DbContext.SaveChangesAsync();
        return registration.Id;
    }

    public async Task UpdateAsync(Registration registration)
    {
        DbContext.Registrations.Update(registration);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var registration = await DbContext.Registrations.FindAsync(id);
        if (registration != null)
        {
            DbContext.Registrations.Remove(registration);
            await DbContext.SaveChangesAsync();
        }
    }
}

public class EfMatchParticipantRepository : EfRepositoryBase, IMatchParticipantRepository
{
    public EfMatchParticipantRepository(TableTennisTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<MatchParticipant>> GetAllAsync()
    {
        return await DbContext.MatchParticipants
            .AsNoTracking()
            .Include(mp => mp.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .Include(mp => mp.Player)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchParticipant>> SearchAsync(string query, string? filter = null)
    {
        var lowerQuery = query.ToLower();
        var participants = DbContext.MatchParticipants
            .AsNoTracking()
            .Include(mp => mp.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .Include(mp => mp.Player)
            .AsSplitQuery()
            .AsQueryable();

        participants = ApplyMatchParticipantFilter(participants, lowerQuery, filter);
        return await participants.ToListAsync();
    }

    public async Task<MatchParticipant?> GetByIdAsync(Guid id)
    {
        return await DbContext.MatchParticipants
            .AsNoTracking()
            .Include(mp => mp.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .Include(mp => mp.Player)
            .AsSplitQuery()
            .FirstOrDefaultAsync(mp => mp.Id == id);
    }

    public async Task<IEnumerable<MatchParticipant>> GetByMatchIdAsync(Guid matchId)
    {
        return await DbContext.MatchParticipants
            .AsNoTracking()
            .Include(mp => mp.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .Include(mp => mp.Player)
            .AsSplitQuery()
            .Where(mp => mp.MatchId == matchId)
            .ToListAsync();
    }

    public async Task<Guid> CreateAsync(MatchParticipant matchParticipant)
    {
        matchParticipant.Id = Guid.NewGuid();
        DbContext.MatchParticipants.Add(matchParticipant);
        await DbContext.SaveChangesAsync();
        return matchParticipant.Id;
    }

    public async Task UpdateAsync(MatchParticipant matchParticipant)
    {
        DbContext.MatchParticipants.Update(matchParticipant);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var matchParticipant = await DbContext.MatchParticipants.FindAsync(id);
        if (matchParticipant != null)
        {
            DbContext.MatchParticipants.Remove(matchParticipant);
            await DbContext.SaveChangesAsync();
        }
    }
}

public class EfMatchSetResultRepository : EfRepositoryBase, IMatchSetResultRepository
{
    public EfMatchSetResultRepository(TableTennisTrackerDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<MatchSetResult>> GetAllAsync()
    {
        return await DbContext.MatchSetResults
            .AsNoTracking()
            .Include(sr => sr.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchSetResult>> SearchAsync(string query, string? filter = null)
    {
        var lowerQuery = query.ToLower();
        var setResults = DbContext.MatchSetResults
            .AsNoTracking()
            .Include(sr => sr.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .AsQueryable();

        setResults = ApplyMatchSetResultFilter(setResults, lowerQuery, filter);
        return await setResults.ToListAsync();
    }

    public async Task<MatchSetResult?> GetByIdAsync(Guid id)
    {
        return await DbContext.MatchSetResults
            .AsNoTracking()
            .Include(sr => sr.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public async Task<IEnumerable<MatchSetResult>> GetByMatchIdAsync(Guid matchId)
    {
        return await DbContext.MatchSetResults
            .AsNoTracking()
            .Include(sr => sr.Match)
                .ThenInclude(m => m.Tournament)
                    .ThenInclude(t => t.Venue)
            .AsSplitQuery()
            .Where(sr => sr.MatchId == matchId)
            .ToListAsync();
    }

    public async Task<Guid> CreateAsync(MatchSetResult matchSetResult)
    {
        matchSetResult.Id = Guid.NewGuid();
        DbContext.MatchSetResults.Add(matchSetResult);
        await DbContext.SaveChangesAsync();
        return matchSetResult.Id;
    }

    public async Task UpdateAsync(MatchSetResult matchSetResult)
    {
        DbContext.MatchSetResults.Update(matchSetResult);
        await DbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var matchSetResult = await DbContext.MatchSetResults.FindAsync(id);
        if (matchSetResult != null)
        {
            DbContext.MatchSetResults.Remove(matchSetResult);
            await DbContext.SaveChangesAsync();
        }
    }
}
