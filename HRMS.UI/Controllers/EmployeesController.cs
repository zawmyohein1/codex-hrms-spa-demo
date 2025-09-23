using Microsoft.AspNetCore.Mvc;

namespace HRMS.UI.Controllers;

public class EmployeesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
