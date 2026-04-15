using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

public class PlayersController : Controller
{
    private readonly IPlayerRepository _playerRepository;

    public PlayersController(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<IActionResult> Index()
    {
        var players = await _playerRepository.GetAllAsync();
        return View(players);
    }
}
