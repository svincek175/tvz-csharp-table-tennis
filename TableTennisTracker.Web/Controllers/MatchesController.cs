using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

public class MatchesController : Controller
{
    private readonly IMatchRepository _matchRepository;

    public MatchesController(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    public async Task<IActionResult> Index()
    {
        var matches = await _matchRepository.GetAllAsync();
        return View(matches);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null)
        {
            return NotFound();
        }

        return View(match);
    }
}
