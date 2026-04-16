using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;
using System.Threading.Tasks;

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
        var tournaments = await _tournamentRepository.GetAllAsync();
        var venues = await _venueRepository.GetAllAsync();
        var players = await _playerRepository.GetAllAsync();
        var matches = await _matchRepository.GetAllAsync();
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
            LatestPlayer = tournaments.FirstOrDefault() != null ? $"{tournaments.First().Name}" : null
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
}
