using System.ComponentModel.DataAnnotations;

namespace MipWeb.Models
{
    public class ContactUsViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; }

        [Phone]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [Display(Name = "I am not a robot")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please confirm you're not a robot.")]
        public bool IsHuman { get; set; }
    }
}
