using System.Diagnostics;
using ClientApp.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using ClientApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace ClientApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [CustomAuthorize(Roles = "player")]
    public IActionResult Play()
    {
        var a = Request.Headers;
        return View();
    }
}