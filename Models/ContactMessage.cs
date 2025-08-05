using System;
using System.ComponentModel.DataAnnotations;

namespace MipWeb.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(1000)]
        public string Message { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
