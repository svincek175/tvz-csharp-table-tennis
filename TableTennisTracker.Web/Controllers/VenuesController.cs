using Microsoft.AspNetCore.Mvc;
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
}
