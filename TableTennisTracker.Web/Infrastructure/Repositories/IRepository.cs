using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Infrastructure.Repositories;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<Tournament?> GetByIdAsync(Guid id);
}

public interface IVenueRepository
{
    Task<IEnumerable<Venue>> GetAllAsync();
    Task<Venue?> GetByIdAsync(Guid id);
}

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<Player?> GetByIdAsync(Guid id);
}

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetAllAsync();
    Task<Match?> GetByIdAsync(Guid id);
    Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid tournamentId);
}

public interface IRegistrationRepository
{
    Task<IEnumerable<Registration>> GetAllAsync();
    Task<Registration?> GetByIdAsync(Guid id);
    Task<IEnumerable<Registration>> GetByTournamentIdAsync(Guid tournamentId);
}

public interface IMatchParticipantRepository
{
    Task<IEnumerable<MatchParticipant>> GetAllAsync();
    Task<MatchParticipant?> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchParticipant>> GetByMatchIdAsync(Guid matchId);
}

public interface IMatchSetResultRepository
{
    Task<IEnumerable<MatchSetResult>> GetAllAsync();
    Task<MatchSetResult?> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchSetResult>> GetByMatchIdAsync(Guid matchId);
}
