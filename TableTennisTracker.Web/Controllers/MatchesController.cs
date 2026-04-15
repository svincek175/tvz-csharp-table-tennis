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
}
