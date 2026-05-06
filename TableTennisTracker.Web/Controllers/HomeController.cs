using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;
using TableTennisTracker.Domain.Models;
using System.Threading.Tasks;
using System.Linq;

namespace TableTennisTracker.Web.Controllers;

public class HomeController : Controller
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IMatchParticipantRepository _matchParticipantRepository;
    private readonly IMatchSetResultRepository _matchSetResultRepository;

    public HomeController(
        ITournamentRepository tournamentRepository,
        IVenueRepository venueRepository,
        IPlayerRepository playerRepository,
        IMatchRepository matchRepository,
        IRegistrationRepository registrationRepository,
        IMatchParticipantRepository matchParticipantRepository,
        IMatchSetResultRepository matchSetResultRepository)
    {
        _tournamentRepository = tournamentRepository;
        _venueRepository = venueRepository;
        _playerRepository = playerRepository;
        _matchRepository = matchRepository;
        _registrationRepository = registrationRepository;
        _matchParticipantRepository = matchParticipantRepository;
        _matchSetResultRepository = matchSetResultRepository;
    }

    public async Task<IActionResult> Index()
    {
        var tournaments = (await _tournamentRepository.GetAllAsync()).OrderByDescending(t => t.StartUtc).ToList();
        var venues = await _venueRepository.GetAllAsync();
        var players = (await _playerRepository.GetAllAsync()).OrderByDescending(p => p.CurrentRankingPoints).ToList();
        var matches = (await _matchRepository.GetAllAsync()).OrderByDescending(m => m.ScheduledStartUtc).ToList();
        var registrations = await _registrationRepository.GetAllAsync();
        var matchParticipants = await _matchParticipantRepository.GetAllAsync();
        var matchSetResults = await _matchSetResultRepository.GetAllAsync();

        var dashboard = new DashboardViewModel
        {
            TournamentCount = tournaments.Count(),
            VenueCount = venues.Count(),
            PlayerCount = players.Count(),
            MatchCount = matches.Count(),
            RegistrationCount = registrations.Count(),
            MatchParticipantCount = matchParticipants.Count(),
            MatchSetResultCount = matchSetResults.Count(),
            LatestTournament = tournaments.FirstOrDefault()?.Name,
            LatestVenue = venues.FirstOrDefault()?.Name,
            LatestPlayer = players.FirstOrDefault() != null ? $"{players.First().FirstName} {players.First().LastName}" : null,
            UpcomingTournaments = tournaments.Take(5).ToList(),
            TopPlayers = players.Take(5).ToList(),
            RecentMatches = matches.Take(5).ToList()
        };

        return View(dashboard);
    }
}

public class DashboardViewModel
{
    public int TournamentCount { get; set; }
    public int VenueCount { get; set; }
    public int PlayerCount { get; set; }
    public int MatchCount { get; set; }
    public int RegistrationCount { get; set; }
    public int MatchParticipantCount { get; set; }
    public int MatchSetResultCount { get; set; }
    public string LatestTournament { get; set; }
    public string LatestVenue { get; set; }
    public string LatestPlayer { get; set; }
    public List<Tournament> UpcomingTournaments { get; set; } = new();
    public List<Player> TopPlayers { get; set; } = new();
    public List<Match> RecentMatches { get; set; } = new();
}
