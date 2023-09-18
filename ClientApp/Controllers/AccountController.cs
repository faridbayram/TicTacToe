using Core.Models;
using Core.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

[Route("account")]
[ApiController]
public class AccountController : Controller
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService loginService)
    {
        _authService = loginService;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginRequestModel requestModel)
    {
        var result = await _authService.LoginAsync(requestModel);
        if (result.IsSucceeded)
        {
            HttpContext.Session.SetString("AuthToken", result.Data.AccessToken.Token);
            
            var user = result.Data.User;
            
            TempData["NickName"] = user.NickName;
            TempData["Id"] = user.Id;
            TempData["Bonuses"] = user.Bonuses;
            
            return RedirectToAction("Play", "Home");
        }

        return View(requestModel);
    }

    [HttpGet("register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterRequestModel requestModel)
    {
        var result = await _authService.RegisterAsync(requestModel);
        
        if (result.IsSucceeded)
        {
            HttpContext.Session.SetString("AuthToken", result.Data.AccessToken.Token);

            var user = result.Data.User;
            
            TempData["NickName"] = user.NickName;
            TempData["Id"] = user.Id;
            TempData["Bonuses"] = user.Bonuses;
            
            return RedirectToAction("Play", "Home");
        }

        return BadRequest("error");
    }
}