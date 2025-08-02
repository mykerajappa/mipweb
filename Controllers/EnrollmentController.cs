using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MipWeb.Data;
using MipWeb.Models;

namespace MipWeb.Controllers;

public class EnrollmentController : Controller
{
    private readonly MipWebContext _context;
    private readonly IWebHostEnvironment _environment;

    public EnrollmentController(MipWebContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var model = new StudentEnrollmentViewModel
        {
            CaptchaQuestion = GenerateCaptcha(HttpContext)
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index(StudentEnrollmentViewModel model)
    {
        if (!ModelState.IsValid || !VerifyCaptcha(HttpContext, model.CaptchaAnswer))
        {
            model.CaptchaQuestion = GenerateCaptcha(HttpContext);
            if (!VerifyCaptcha(HttpContext, model.CaptchaAnswer))
                ModelState.AddModelError("CaptchaAnswer", "Incorrect captcha.");
            return View(model);
        }

        if (!ValidateFile(model.AadhaarFile) || !ValidateFile(model.PhotoFile))
        {
            ModelState.AddModelError(string.Empty, "Only JPG/PNG files ≤ 1MB allowed.");
            model.CaptchaQuestion = GenerateCaptcha(HttpContext);
            return View(model);
        }

        // Check for existing student
        var duplicate = await _context.Students.FirstOrDefaultAsync(s =>
            s.Phone == model.Phone &&
            s.Name.ToLower() == model.Name.ToLower().Trim() &&
            s.Gender == model.Gender &&
            s.Age == model.Age
        );

        if (duplicate != null)
        {
            ModelState.AddModelError("", "You have already enrolled. Please wait for approval.");
            return View(model);
        }
        
        try
        {

            // Save files
            var aadhaarPath = await SaveFile(model.AadhaarFile, "aadhaar");
            var photoPath = await SaveFile(model.PhotoFile, "photos");

            // Create student record
            var student = new Student
            {
                Name = model.Name,
                Nationality = model.Nationality,
                Gender = model.Gender,
                PreferredLanguage = model.PreferredLanguage,
                Phone = model.Phone,
                Age = model.Age,
                AadhaarFilePath = aadhaarPath,
                PhotoFilePath = photoPath,
                EnrollmentDate = DateTime.Now,
                IsWhatsAppVerified = false,
                IsApproved = false
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Enrollment submitted successfully. Awaiting admin approval.";
        }
        catch (Exception)
        {
            // Set a global error message
            ViewBag.EnrollmentError = "Something went wrong during enrollment. Please try again.";
            return View(model);
        }

        return RedirectToAction("Index");
    }

    private async Task<string> SaveFile(IFormFile file, string folder)
    {
        var uploads = Path.Combine(_environment.WebRootPath, "uploads", folder);
        Directory.CreateDirectory(uploads);
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploads, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/{folder}/{fileName}";
    }

    private bool ValidateFile(IFormFile file)
    {
        var validTypes = new[] { "image/jpeg", "image/png" };
        return file.Length <= 1_000_000 && validTypes.Contains(file.ContentType);
    }

    private string GenerateCaptcha(HttpContext context)
    {
        var rnd = new Random();
        int a = rnd.Next(1, 10), b = rnd.Next(1, 10);
        context.Session.SetInt32("CaptchaAnswer", a + b);
        return $"What is {a} + {b}?";
    }

    private bool VerifyCaptcha(HttpContext context, string answer)
    {
        int? expected = context.Session.GetInt32("CaptchaAnswer");
        return expected.HasValue && answer == expected.Value.ToString();
    }
}
