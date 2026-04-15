using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

public class VenuesController : Controller
{
    private readonly IVenueRepository _venueRepository;

    public VenuesController(IVenueRepository venueRepository)
    {
        _venueRepository = venueRepository;
    }

    public async Task<IActionResult> Index()
    {
        var venues = await _venueRepository.GetAllAsync();
        return View(venues);
    }
}
