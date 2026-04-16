using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

public class MatchParticipantsController : Controller
{
    private readonly IMatchParticipantRepository _matchParticipantRepository;

    public MatchParticipantsController(IMatchParticipantRepository matchParticipantRepository)
    {
        _matchParticipantRepository = matchParticipantRepository;
    }

    public async Task<IActionResult> Index()
    {
        var participants = await _matchParticipantRepository.GetAllAsync();
        return View(participants);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var participant = await _matchParticipantRepository.GetByIdAsync(id);
        if (participant is null)
        {
            return NotFound();
        }

        return View(participant);
    }
}
