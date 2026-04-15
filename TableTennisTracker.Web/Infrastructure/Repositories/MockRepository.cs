using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Infrastructure.Repositories;

public class MockRepository
{
    private static readonly Lazy<(List<Venue>, List<Tournament>, List<Player>, List<Registration>, List<Match>, List<MatchParticipant>, List<MatchSetResult>)> DataCache
        = new(() => DataSeeder.SeedData());

    protected static (List<Venue>, List<Tournament>, List<Player>, List<Registration>, List<Match>, List<MatchParticipant>, List<MatchSetResult>) Data => DataCache.Value;
}

public class MockTournamentRepository : MockRepository, ITournamentRepository
{
    public Task<IEnumerable<Tournament>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Tournament>>(Data.Item2);
    }

    public Task<Tournament?> GetByIdAsync(Guid id)
    {
        var tournament = Data.Item2.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tournament);
    }
}

public class MockVenueRepository : MockRepository, IVenueRepository
{
    public Task<IEnumerable<Venue>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Venue>>(Data.Item1);
    }

    public Task<Venue?> GetByIdAsync(Guid id)
    {
        var venue = Data.Item1.FirstOrDefault(v => v.Id == id);
        return Task.FromResult(venue);
    }
}

public class MockPlayerRepository : MockRepository, IPlayerRepository
{
    public Task<IEnumerable<Player>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Player>>(Data.Item3);
    }

    public Task<Player?> GetByIdAsync(Guid id)
    {
        var player = Data.Item3.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(player);
    }
}

public class MockMatchRepository : MockRepository, IMatchRepository
{
    public Task<IEnumerable<Match>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Match>>(Data.Item5);
    }

    public Task<Match?> GetByIdAsync(Guid id)
    {
        var match = Data.Item5.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(match);
    }

    public Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid tournamentId)
    {
        var tournamentMatches = Data.Item5.Where(m => m.TournamentId == tournamentId).ToList();
        return Task.FromResult<IEnumerable<Match>>(tournamentMatches);
    }
}

public class MockRegistrationRepository : MockRepository, IRegistrationRepository
{
    public Task<IEnumerable<Registration>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Registration>>(Data.Item4);
    }

    public Task<Registration?> GetByIdAsync(Guid id)
    {
        var registration = Data.Item4.FirstOrDefault(r => r.Id == id);
        return Task.FromResult(registration);
    }

    public Task<IEnumerable<Registration>> GetByTournamentIdAsync(Guid tournamentId)
    {
        var tournamentRegistrations = Data.Item4.Where(r => r.TournamentId == tournamentId).ToList();
        return Task.FromResult<IEnumerable<Registration>>(tournamentRegistrations);
    }
}

public class MockMatchParticipantRepository : MockRepository, IMatchParticipantRepository
{
    public Task<IEnumerable<MatchParticipant>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<MatchParticipant>>(Data.Item6);
    }

    public Task<MatchParticipant?> GetByIdAsync(Guid id)
    {
        var participant = Data.Item6.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(participant);
    }

    public Task<IEnumerable<MatchParticipant>> GetByMatchIdAsync(Guid matchId)
    {
        var matchParticipants = Data.Item6.Where(p => p.MatchId == matchId).ToList();
        return Task.FromResult<IEnumerable<MatchParticipant>>(matchParticipants);
    }
}

public class MockMatchSetResultRepository : MockRepository, IMatchSetResultRepository
{
    public Task<IEnumerable<MatchSetResult>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<MatchSetResult>>(Data.Item7);
    }

    public Task<MatchSetResult?> GetByIdAsync(Guid id)
    {
        var setResult = Data.Item7.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(setResult);
    }

    public Task<IEnumerable<MatchSetResult>> GetByMatchIdAsync(Guid matchId)
    {
        var matchSetResults = Data.Item7.Where(s => s.MatchId == matchId).ToList();
        return Task.FromResult<IEnumerable<MatchSetResult>>(matchSetResults);
    }
}
