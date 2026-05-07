using Microsoft.AspNetCore.Mvc;
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
}
