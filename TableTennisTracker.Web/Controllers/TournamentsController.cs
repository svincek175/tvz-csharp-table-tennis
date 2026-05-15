using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Repositories;
using TableTennisTracker.Web.Infrastructure;

namespace TableTennisTracker.Web.Controllers;

[Route("tournaments")]
public class TournamentsController : Controller
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IVenueRepository _venueRepository;

    public TournamentsController(ITournamentRepository tournamentRepository, IVenueRepository venueRepository)
    {
        _tournamentRepository = tournamentRepository;
        _venueRepository = venueRepository;
    }

    [HttpGet("list")]
    public async Task<IActionResult> Index()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return View(tournaments);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allTournaments = await _tournamentRepository.GetAllAsync();
            return PartialView("_TournamentsList", allTournaments);
        }

        var tournaments = await _tournamentRepository.SearchAsync(query);
        return PartialView("_TournamentsList", tournaments);
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(string query)
    {
        var tournaments = string.IsNullOrWhiteSpace(query)
            ? await _tournamentRepository.GetAllAsync()
            : await _tournamentRepository.SearchAsync(query);

        var items = tournaments
            .Take(10)
            .Select(tournament => new TableTennisTracker.Web.ViewModels.AutocompleteLookupItem(
                tournament.Id,
                tournament.Name,
                $"{tournament.SeasonLabel} · {tournament.StartUtc:yyyy-MM-dd}"));

        return Json(items);
    }

    [HttpGet("view/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament is null)
        {
            return NotFound();
        }

        return View(tournament);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Venues = await _venueRepository.GetAllAsync();
        return View(new Tournament());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tournament tournament)
    {
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Tournament.VenueId), tournament.VenueId, "Venue is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Venues = await _venueRepository.GetAllAsync();
            return View(tournament);
        }

        var id = await _tournamentRepository.CreateAsync(tournament);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament is null)
        {
            return NotFound();
        }

        ViewBag.Venues = await _venueRepository.GetAllAsync();
        return View(tournament);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Tournament tournament)
    {
        if (id != tournament.Id)
        {
            return BadRequest();
        }

        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Tournament.VenueId), tournament.VenueId, "Venue is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Venues = await _venueRepository.GetAllAsync();
            return View(tournament);
        }

        await _tournamentRepository.UpdateAsync(tournament);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament is null)
        {
            return NotFound();
        }

        return View(tournament);
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _tournamentRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
