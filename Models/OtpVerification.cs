namespace MipWeb.Models;
public class OtpVerification
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; }
    public string OtpCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsVerified { get; set; }
}