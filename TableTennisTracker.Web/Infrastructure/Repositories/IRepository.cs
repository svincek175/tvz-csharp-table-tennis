using TableTennisTracker.Domain.Models;

namespace TableTennisTracker.Web.Infrastructure.Repositories;

public interface ITournamentRepository
{
    Task<IEnumerable<Tournament>> GetAllAsync();
    Task<IEnumerable<Tournament>> SearchAsync(string query, string? filter = null);
    Task<Tournament?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Tournament tournament);
    Task UpdateAsync(Tournament tournament);
    Task DeleteAsync(Guid id);
}

public interface IVenueRepository
{
    Task<IEnumerable<Venue>> GetAllAsync();
    Task<IEnumerable<Venue>> SearchAsync(string query, string? filter = null);
    Task<Venue?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Venue venue);
    Task UpdateAsync(Venue venue);
    Task DeleteAsync(Guid id);
}

public interface IPlayerRepository
{
    Task<IEnumerable<Player>> GetAllAsync();
    Task<IEnumerable<Player>> SearchAsync(string query, string? filter = null);
    Task<Player?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Player player);
    Task UpdateAsync(Player player);
    Task DeleteAsync(Guid id);
}

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetAllAsync();
    Task<IEnumerable<Match>> SearchAsync(string query, string? filter = null);
    Task<Match?> GetByIdAsync(Guid id);
    Task<IEnumerable<Match>> GetByTournamentIdAsync(Guid tournamentId);
    Task<Guid> CreateAsync(Match match);
    Task UpdateAsync(Match match);
    Task DeleteAsync(Guid id);
}

public interface IRegistrationRepository
{
    Task<IEnumerable<Registration>> GetAllAsync();
    Task<IEnumerable<Registration>> SearchAsync(string query, string? filter = null);
    Task<Registration?> GetByIdAsync(Guid id);
    Task<IEnumerable<Registration>> GetByTournamentIdAsync(Guid tournamentId);
    Task<Guid> CreateAsync(Registration registration);
    Task UpdateAsync(Registration registration);
    Task DeleteAsync(Guid id);
}

public interface IMatchParticipantRepository
{
    Task<IEnumerable<MatchParticipant>> GetAllAsync();
    Task<IEnumerable<MatchParticipant>> SearchAsync(string query, string? filter = null);
    Task<MatchParticipant?> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchParticipant>> GetByMatchIdAsync(Guid matchId);
    Task<Guid> CreateAsync(MatchParticipant matchParticipant);
    Task UpdateAsync(MatchParticipant matchParticipant);
    Task DeleteAsync(Guid id);
}

public interface IMatchSetResultRepository
{
    Task<IEnumerable<MatchSetResult>> GetAllAsync();
    Task<IEnumerable<MatchSetResult>> SearchAsync(string query, string? filter = null);
    Task<MatchSetResult?> GetByIdAsync(Guid id);
    Task<IEnumerable<MatchSetResult>> GetByMatchIdAsync(Guid matchId);
    Task<Guid> CreateAsync(MatchSetResult matchSetResult);
    Task UpdateAsync(MatchSetResult matchSetResult);
    Task DeleteAsync(Guid id);
}
