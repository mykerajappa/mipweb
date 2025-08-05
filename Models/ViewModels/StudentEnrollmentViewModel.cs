using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MipWeb.Models;

using System.ComponentModel.DataAnnotations;

public class StudentEnrollmentViewModel
{
    [Required, StringLength(100)]
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name must contain only letters.")]
    public string Name { get; set; } = "";

    [Required, StringLength(50)]
    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Nationality must contain only letters.")]
    public string Nationality { get; set; } = "";

    [Required]
    public string Gender { get; set; } = "";

    [Required]
    public string PreferredLanguage { get; set; } = "";

    [Required]
    [Display(Name = "Phone (WhatsApp enabled)")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit phone number.")]
    public string Phone { get; set; } = "";

    [Required, Range(5, 70)]
    public int Age { get; set; }

    [Required(ErrorMessage = "Captcha answer is required.")]
    public string CaptchaAnswer { get; set; } = "";

    [Required]
    [Display(Name = "Aadhaar (JPG/PNG ≤ 1MB)")]
    public IFormFile AadhaarFile { get; set; } = null!;

    [Required]
    [Display(Name = "Photo (JPG/PNG ≤ 1MB)")]
    public IFormFile PhotoFile { get; set; } = null!;

    public string CaptchaQuestion { get; set; } = "";
}
