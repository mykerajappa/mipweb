using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MipWeb.Data;
using MipWeb.Models;

namespace MipWeb.Controllers;
public class LoginController : Controller
{
    private readonly MipWebContext _context;
    public LoginController(MipWebContext context) => _context = context;

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> Index(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            ViewBag.Error = "Invalid login";
            return View();
        }

        // Create auth cookie
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, "login");
        await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

        return RedirectToAction("PendingApproval", "Admin");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index");
    }
}
