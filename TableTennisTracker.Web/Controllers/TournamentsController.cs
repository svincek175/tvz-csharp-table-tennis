using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

public class TournamentsController : Controller
{
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentsController(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task<IActionResult> Index()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return View(tournaments);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament is null)
        {
            return NotFound();
        }

        return View(tournament);
    }
}
