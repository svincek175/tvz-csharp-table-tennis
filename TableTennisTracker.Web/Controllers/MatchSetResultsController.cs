using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

public class MatchSetResultsController : Controller
{
    private readonly IMatchSetResultRepository _matchSetResultRepository;

    public MatchSetResultsController(IMatchSetResultRepository matchSetResultRepository)
    {
        _matchSetResultRepository = matchSetResultRepository;
    }

    public async Task<IActionResult> Index()
    {
        var setResults = await _matchSetResultRepository.GetAllAsync();
        return View(setResults);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var setResult = await _matchSetResultRepository.GetByIdAsync(id);
        if (setResult is null)
        {
            return NotFound();
        }

        return View(setResult);
    }
}
