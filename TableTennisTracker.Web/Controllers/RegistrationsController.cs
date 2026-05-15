using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Repositories;
using TableTennisTracker.Web.Infrastructure;

namespace TableTennisTracker.Web.Controllers;

[Route("registrations")]
public class RegistrationsController : Controller
{
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IPlayerRepository _playerRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public RegistrationsController(IRegistrationRepository registrationRepository, IPlayerRepository playerRepository, ITournamentRepository tournamentRepository)
    {
        _registrationRepository = registrationRepository;
        _playerRepository = playerRepository;
        _tournamentRepository = tournamentRepository;
    }

    [HttpGet("current")]
    public async Task<IActionResult> Index()
    {
        var registrations = await _registrationRepository.GetAllAsync();
        return View(registrations);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allRegistrations = await _registrationRepository.GetAllAsync();
            return PartialView("_RegistrationsList", allRegistrations);
        }

        var registrations = await _registrationRepository.SearchAsync(query);
        return PartialView("_RegistrationsList", registrations);
    }

    [HttpGet("info/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var registration = await _registrationRepository.GetByIdAsync(id);
        if (registration is null)
        {
            return NotFound();
        }

        return View(registration);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Players = await _playerRepository.GetAllAsync();
        ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
        return View();
    }

    [HttpGet("players-lookup")]
    public async Task<IActionResult> PlayersLookup(string query)
    {
        var players = string.IsNullOrWhiteSpace(query)
            ? await _playerRepository.GetAllAsync()
            : await _playerRepository.SearchAsync(query);

        var items = players
            .Take(10)
            .Select(player => new TableTennisTracker.Web.ViewModels.AutocompleteLookupItem(
                player.Id,
                $"{player.FirstName} {player.LastName}",
                player.CountryCode));

        return Json(items);
    }

    [HttpGet("tournaments-lookup")]
    public async Task<IActionResult> TournamentsLookup(string query)
    {
        var tournaments = string.IsNullOrWhiteSpace(query)
            ? await _tournamentRepository.GetAllAsync()
            : await _tournamentRepository.SearchAsync(query);

        var items = tournaments
            .Take(10)
            .Select(tournament => new TableTennisTracker.Web.ViewModels.AutocompleteLookupItem(
                tournament.Id,
                tournament.Name,
                tournament.SeasonLabel));

        return Json(items);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Registration registration)
    {
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Registration.PlayerId), registration.PlayerId, "Player is required.");
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Registration.TournamentId), registration.TournamentId, "Tournament is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Players = await _playerRepository.GetAllAsync();
            ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
            return View(registration);
        }

        var id = await _registrationRepository.CreateAsync(registration);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var registration = await _registrationRepository.GetByIdAsync(id);
        if (registration is null)
        {
            return NotFound();
        }

        ViewBag.Players = await _playerRepository.GetAllAsync();
        ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
        return View(registration);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Registration registration)
    {
        if (id != registration.Id)
        {
            return BadRequest();
        }

        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Registration.PlayerId), registration.PlayerId, "Player is required.");
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(Registration.TournamentId), registration.TournamentId, "Tournament is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Players = await _playerRepository.GetAllAsync();
            ViewBag.Tournaments = await _tournamentRepository.GetAllAsync();
            return View(registration);
        }

        await _registrationRepository.UpdateAsync(registration);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var registration = await _registrationRepository.GetByIdAsync(id);
        if (registration is null)
        {
            return NotFound();
        }

        return View(registration);
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _registrationRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
