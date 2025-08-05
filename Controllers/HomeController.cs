using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MipWeb.Data;
using MipWeb.Models;

namespace MipWeb.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MipWebContext _context;

    public HomeController(MipWebContext context, ILogger<HomeController> logger)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View(new ContactUsViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Contact(ContactUsViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);


        var message = new ContactMessage
        {
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            Message = model.Message,
            SubmittedAt = DateTime.Now
        };

        _context.ContactMessages.Add(message);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Thank you for contacting us. We'll be in touch!";
        return RedirectToAction("Contact");
    }
}
