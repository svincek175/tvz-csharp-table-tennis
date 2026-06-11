using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Repositories;
using TableTennisTracker.Web.Infrastructure;

namespace TableTennisTracker.Web.Controllers;

[Route("match-set-results")]
public class MatchSetResultsController : Controller
{
    private readonly IMatchSetResultRepository _matchSetResultRepository;
    private readonly IMatchRepository _matchRepository;

    public MatchSetResultsController(IMatchSetResultRepository matchSetResultRepository, IMatchRepository matchRepository)
    {
        _matchSetResultRepository = matchSetResultRepository;
        _matchRepository = matchRepository;
    }

    [HttpGet("list")]
    public async Task<IActionResult> Index()
    {
        var setResults = await _matchSetResultRepository.GetAllAsync();
        return View(setResults);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query, string? filter = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allResults = await _matchSetResultRepository.GetAllAsync();
            return PartialView("_MatchSetResultsList", allResults);
        }

        var setResults = await _matchSetResultRepository.SearchAsync(query, filter);
        return PartialView("_MatchSetResultsList", setResults);
    }

    [HttpGet("matches-lookup")]
    public async Task<IActionResult> MatchesLookup(string query)
    {
        var matches = string.IsNullOrWhiteSpace(query)
            ? await _matchRepository.GetAllAsync()
            : await _matchRepository.SearchAsync(query);

        var items = matches
            .Take(10)
            .Select(match => new TableTennisTracker.Web.ViewModels.AutocompleteLookupItem(
                match.Id,
                match.Tournament?.Name ?? "Match",
                $"Round {match.RoundNumber} · Table {match.TableNumber}"));

        return Json(items);
    }

    [HttpGet("view/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var setResult = await _matchSetResultRepository.GetByIdAsync(id);
        if (setResult is null)
        {
            return NotFound();
        }

        return View(setResult);
    }

    [HttpGet("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Matches = await _matchRepository.GetAllAsync();
        return View();
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(MatchSetResult matchSetResult)
    {
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(MatchSetResult.MatchId), matchSetResult.MatchId, "Match is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Matches = await _matchRepository.GetAllAsync();
            return View(matchSetResult);
        }

        var id = await _matchSetResultRepository.CreateAsync(matchSetResult);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("edit/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var setResult = await _matchSetResultRepository.GetByIdAsync(id);
        if (setResult is null)
        {
            return NotFound();
        }

        ViewBag.Matches = await _matchRepository.GetAllAsync();
        return View(setResult);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id, MatchSetResult matchSetResult)
    {
        if (id != matchSetResult.Id)
        {
            return BadRequest();
        }

        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(MatchSetResult.MatchId), matchSetResult.MatchId, "Match is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Matches = await _matchRepository.GetAllAsync();
            return View(matchSetResult);
        }

        await _matchSetResultRepository.UpdateAsync(matchSetResult);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var setResult = await _matchSetResultRepository.GetByIdAsync(id);
        if (setResult is null)
        {
            return NotFound();
        }

        return View(setResult);
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _matchSetResultRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
