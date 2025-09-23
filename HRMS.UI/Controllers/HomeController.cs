using Microsoft.AspNetCore.Mvc;

namespace HRMS.UI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Dashboard";
        return View();
    }
}
