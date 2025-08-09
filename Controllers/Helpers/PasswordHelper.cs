namespace MipWeb.Helpers;

public static class PasswordHelper
{
    private static Random random = new();

    public static string GenerateSecurePassword()
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digit = "0123456789";
        const string special = "#$%@";

        var all = upper + lower + digit + special;
        var rnd = new Random();
        return new string(Enumerable.Range(1, rnd.Next(8, 16)).Select(_ => all[rnd.Next(all.Length)]).ToArray());
    }

    public static string Hash(string password)
    {
        // Use a proper hash method in real code
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
