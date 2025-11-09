using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UnlockSecretApp.Models;

namespace UnlockSecretApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private const string SecretCode = "1234"; // твой 4-значный код

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult CheckCode(string code)
    {
        if (code == SecretCode)
            return RedirectToAction("Gift");
        else
        {
            ViewBag.Error = "Неверный код 😅";
            return View("Index");
        }
    }

    public IActionResult Gift() => View();
}

