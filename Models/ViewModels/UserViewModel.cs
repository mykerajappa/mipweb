using System.ComponentModel.DataAnnotations;

namespace MipWeb.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string WhatsappNumber { get; set; }

        public List<string> AllRoles { get; set; } = new List<string>();
        public List<string> SelectedRoles { get; set; } = new List<string>();

        public bool IsActive { get; set; } = true;
    }

}
