using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Domain.Models;
using TableTennisTracker.Web.Infrastructure.Repositories;
using TableTennisTracker.Web.Infrastructure;

namespace TableTennisTracker.Web.Controllers;

[Route("match-participants")]
public class MatchParticipantsController : Controller
{
    private readonly IMatchParticipantRepository _matchParticipantRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IPlayerRepository _playerRepository;

    public MatchParticipantsController(IMatchParticipantRepository matchParticipantRepository, IMatchRepository matchRepository, IPlayerRepository playerRepository)
    {
        _matchParticipantRepository = matchParticipantRepository;
        _matchRepository = matchRepository;
        _playerRepository = playerRepository;
    }

    [HttpGet("list")]
    public async Task<IActionResult> Index()
    {
        var participants = await _matchParticipantRepository.GetAllAsync();
        return View(participants);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            var allParticipants = await _matchParticipantRepository.GetAllAsync();
            return PartialView("_MatchParticipantsList", allParticipants);
        }

        var participants = await _matchParticipantRepository.SearchAsync(query);
        return PartialView("_MatchParticipantsList", participants);
    }

    [HttpGet("view/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var participant = await _matchParticipantRepository.GetByIdAsync(id);
        if (participant is null)
        {
            return NotFound();
        }

        return View(participant);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Matches = await _matchRepository.GetAllAsync();
        ViewBag.Players = await _playerRepository.GetAllAsync();
        return View();
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MatchParticipant matchParticipant)
    {
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(MatchParticipant.MatchId), matchParticipant.MatchId, "Match is required.");
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(MatchParticipant.PlayerId), matchParticipant.PlayerId, "Player is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Matches = await _matchRepository.GetAllAsync();
            ViewBag.Players = await _playerRepository.GetAllAsync();
            return View(matchParticipant);
        }

        var id = await _matchParticipantRepository.CreateAsync(matchParticipant);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("edit/{id:guid}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var participant = await _matchParticipantRepository.GetByIdAsync(id);
        if (participant is null)
        {
            return NotFound();
        }

        ViewBag.Matches = await _matchRepository.GetAllAsync();
        ViewBag.Players = await _playerRepository.GetAllAsync();
        return View(participant);
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, MatchParticipant matchParticipant)
    {
        if (id != matchParticipant.Id)
        {
            return BadRequest();
        }

        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(MatchParticipant.MatchId), matchParticipant.MatchId, "Match is required.");
        ValidationHelper.EnsureNotEmptyGuid(ModelState, nameof(MatchParticipant.PlayerId), matchParticipant.PlayerId, "Player is required.");

        if (!ModelState.IsValid)
        {
            ViewBag.Matches = await _matchRepository.GetAllAsync();
            ViewBag.Players = await _playerRepository.GetAllAsync();
            return View(matchParticipant);
        }

        await _matchParticipantRepository.UpdateAsync(matchParticipant);
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet("delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var participant = await _matchParticipantRepository.GetByIdAsync(id);
        if (participant is null)
        {
            return NotFound();
        }

        return View(participant);
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        await _matchParticipantRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
