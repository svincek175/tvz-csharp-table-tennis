using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

[Route("venues")]
public class VenuesController : Controller
{
    private readonly IVenueRepository _venueRepository;

    public VenuesController(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> Index()
    {
        var venues = await _venueRepository.GetAllAsync();
        return View(venues);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query, string? filter = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allVenues = await _venueRepository.GetAllAsync();
            return PartialView("_VenuesList", allVenues);
        }

        var venues = await _venueRepository.SearchAsync(query, filter);
        return PartialView("_VenuesList", venues);
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(string query)
    {
        var venues = string.IsNullOrWhiteSpace(query)
            ? await _venueRepository.GetAllAsync()
            : await _venueRepository.SearchAsync(query);

        var items = venues
            .Take(10)
            .Select(venue => new TableTennisTracker.Web.ViewModels.AutocompleteLookupItem(
                venue.Id,
                venue.Name,
                $"{venue.City} · {venue.CountryCode}"));

        return Json(items);
    }

    [HttpGet("location/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue is null)
        {
            return NotFound();
        }

        return View(venue);
    }

    [HttpGet("create")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Venue venue)
    {
        if (!ModelState.IsValid)
        {
            return View(venue);
        }

        var id = await _venueRepository.CreateAsync(venue);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("edit/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue is null)
        {
            return NotFound();
        }

        return View(venue);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id, Venue venue)
    {
        if (id != venue.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(venue);
        }

        await _venueRepository.UpdateAsync(venue);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var venue = await _venueRepository.GetByIdAsync(id);
        if (venue is null)
        {
            return NotFound();
        }

        return View(venue);
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _venueRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
