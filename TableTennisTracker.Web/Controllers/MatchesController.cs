using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Repositories;
using TableTennisTracker.Web.Infrastructure;

namespace TableTennisTracker.Web.Controllers;

[Route("matches")]
public class MatchesController : Controller
{
    private readonly IMatchRepository _matchRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public MatchesController(IMatchRepository matchRepository, ITournamentRepository tournamentRepository)
    {
        _matchRepository = matchRepository;
        _tournamentRepository = tournamentRepository;
    }

    [HttpGet("all")]
    public async Task<IActionResult> Index()
    {
        var matches = await _matchRepository.GetAllAsync();
        return View(matches);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query, string? filter = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allMatches = await _matchRepository.GetAllAsync();
            return PartialView("_MatchesList", allMatches);
        }

        var matches = await _matchRepository.SearchAsync(query, filter);
        return PartialView("_MatchesList", matches);
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(string query)
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
        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null)
        {
            return NotFound();
        }

        return View(match);
    }

    [HttpGet("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
        return View();
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Match match)
    {
        NormalizeMatchDateTimes(match);
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Match.TournamentId), match.TournamentId, "Tournament is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
            return View(match);
        }

        var id = await _matchRepository.CreateAsync(match);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("edit/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null)
        {
            return NotFound();
        }

        ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
        return View(match);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id, Match match)
    {
        if (id != match.Id)
        {
            return BadRequest();
        }

        NormalizeMatchDateTimes(match);
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Match.TournamentId), match.TournamentId, "Tournament is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
            return View(match);
        }

        await _matchRepository.UpdateAsync(match);
        return RedirectToAction(nameof(Details), new { id });
    }

    private static void NormalizeMatchDateTimes(Match match)
    {
        match.ScheduledStartUtc = DateTime.SpecifyKind(match.ScheduledStartUtc, DateTimeKind.Utc);
        match.ActualStartUtc = match.ActualStartUtc.HasValue
            ? DateTime.SpecifyKind(match.ActualStartUtc.Value, DateTimeKind.Utc)
            : null;
        match.CompletedUtc = match.CompletedUtc.HasValue
            ? DateTime.SpecifyKind(match.CompletedUtc.Value, DateTimeKind.Utc)
            : null;
    }

    [HttpGet("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var match = await _matchRepository.GetByIdAsync(id);
        if (match is null)
        {
            return NotFound();
        }

        return View(match);
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _matchRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
