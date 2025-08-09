using Microsoft.EntityFrameworkCore;
using MipWeb.Models;

namespace MipWeb.Data;

public class MipWebContext : DbContext
{
    public MipWebContext(DbContextOptions<MipWebContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<ExpenseCategory>().HasData(
            new ExpenseCategory { ExpenseCategoryID = 1, CategoryName = "Other", IsActive = true }
        );
    }

    internal IQueryable<User> GetUsersWithRoles()
    {
        return Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);
    }

    internal IQueryable<User> GetPartners()
    {
        return GetUsersWithRoles()
            .Where(u => u.UserRoles.Any(r => r.Role.Name == AppRoles.Partner));
    }
}
   