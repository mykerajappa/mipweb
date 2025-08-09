namespace MipWeb.Models;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Faculty = "Faculty";
    public const string Partner = "Partner";
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Username { get; set; } = null!; // unique text ID
    public string PasswordHash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string WhatsAppNumber { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}


public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;    // Unique
    public string Description { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}


