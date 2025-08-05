using Microsoft.EntityFrameworkCore;
using MipWeb.Models;

namespace MipWeb.Data;

public class MipWebContext : DbContext
{
    public MipWebContext(DbContextOptions<MipWebContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
    public DbSet<ContactMessage> ContactMessages { get; set; }
}
