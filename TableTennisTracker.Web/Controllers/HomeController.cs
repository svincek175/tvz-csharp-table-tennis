using Microsoft.AspNetCore.Mvc;

namespace TableTennisTracker.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
