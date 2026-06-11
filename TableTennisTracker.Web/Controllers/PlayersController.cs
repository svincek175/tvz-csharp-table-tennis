using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

[Route("players")]
public class PlayersController : Controller
{
    private readonly IPlayerRepository _playerRepository;

    public PlayersController(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    [HttpGet("list")]
    public async Task<IActionResult> Index()
    {
        var players = await _playerRepository.GetAllAsync();
        return View(players);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query, string? filter = null)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allPlayers = await _playerRepository.GetAllAsync();
            return PartialView("_PlayersList", allPlayers);
        }

        var players = await _playerRepository.SearchAsync(query, filter);
        return PartialView("_PlayersList", players);
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup(string query)
    {
        var players = string.IsNullOrWhiteSpace(query)
            ? await _playerRepository.GetAllAsync()
            : await _playerRepository.SearchAsync(query);

        var items = players
            .Take(10)
            .Select(player => new TableTennisTracker.Web.ViewModels.AutocompleteLookupItem(
                player.Id,
                $"{player.FirstName} {player.LastName}",
                $"{player.CountryCode} · {player.CurrentRankingPoints} pts"));

        return Json(items);
    }

    [HttpGet("profile/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var player = await _playerRepository.GetByIdAsync(id);
        if (player is null)
        {
            return NotFound();
        }

        return View(player);
    }

    [HttpGet("create")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new Player());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Player player)
    {
        NormalizePlayerDateTimes(player);
        if (!ModelState.IsValid)
        {
            return View(player);
        }

        player.IsActive = true;
        var id = await _playerRepository.CreateAsync(player);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("edit/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var player = await _playerRepository.GetByIdAsync(id);
        if (player is null)
        {
            return NotFound();
        }

        return View(player);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Guid id, Player player)
    {
        if (id != player.Id)
        {
            return BadRequest();
        }

        NormalizePlayerDateTimes(player);
        if (!ModelState.IsValid)
        {
            return View(player);
        }

        await _playerRepository.UpdateAsync(player);
        return RedirectToAction(nameof(Details), new { id });
    }

    private static void NormalizePlayerDateTimes(Player player)
    {
        player.CreatedUtc = DateTime.SpecifyKind(player.CreatedUtc, DateTimeKind.Utc);
    }

    [HttpGet("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var player = await _playerRepository.GetByIdAsync(id);
        if (player is null)
        {
            return NotFound();
        }

        return View(player);
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _playerRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
