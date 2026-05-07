using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

[Route("matches")]
public class MatchesController : Controller
{
    private readonly IMatchRepository _matchRepository;

    public MatchesController(IMatchRepository matchRepository)
    {
        _matchRepository = matchRepository;
    }

    [HttpGet("all")]
    public async Task<IActionResult> Index()
    {
        var matches = await _matchRepository.GetAllAsync();
        return View(matches);
    }

    [HttpGet("view/{id:guid}")]
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
