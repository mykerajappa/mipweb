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
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = AppRoles.Admin; // Only Admin for now
}
