using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Shared.Models.ViewModels;

namespace TechMoveGLMS.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Login", "Account");
        return View();
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel());
}