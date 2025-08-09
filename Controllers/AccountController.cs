using Microsoft.AspNetCore.Mvc;
using MipWeb.Models; // or your namespace
using MipWeb.ViewModels;
using MipWeb.Data;   // for AppDbContext
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly MipWebContext _context;

    public AccountController(MipWebContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var user = _context.GetUsersWithRoles()
                    .FirstOrDefault(u => u.Username == username);

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserID", user.Id.ToString())
            };

            foreach (var ur in user.UserRoles)
                claims.Add(new Claim(ClaimTypes.Role, ur.Role.Name));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync("MipAuth", new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Admin");
        }

        ViewBag.Error = "Invalid login";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MipAuth");
        return RedirectToAction("Login");
    }
}
