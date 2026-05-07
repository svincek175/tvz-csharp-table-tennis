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

    public async Task<Venue?> GetByIdAsync(Guid id)
    {
        return await DbContext.Venues
            .AsNoTracking()
            .Include(v => v.Tournaments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(v => v.Id == id);
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
}
