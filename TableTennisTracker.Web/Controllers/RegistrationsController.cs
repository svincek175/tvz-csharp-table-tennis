using Microsoft.AspNetCore.Mvc;
using TableTennisTracker.Web.Infrastructure.Repositories;

namespace TableTennisTracker.Web.Controllers;

public class RegistrationsController : Controller
{
    private readonly IRegistrationRepository _registrationRepository;

    public RegistrationsController(IRegistrationRepository registrationRepository)
    {
        _registrationRepository = registrationRepository;
    }

    public async Task<IActionResult> Index()
    {
        var registrations = await _registrationRepository.GetAllAsync();
        return View(registrations);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var registration = await _registrationRepository.GetByIdAsync(id);
        if (registration is null)
        {
            return NotFound();
        }

        return View(registration);
    }
}
