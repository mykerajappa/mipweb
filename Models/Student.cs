namespace MipWeb.Models;

public class Student
{
    public int Id { get; set; }  // Internal DB PK
    public string? StudentId { get; set; }  // Null until approved
    public string Name { get; set; } = string.Empty;
    public string Nationality { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty; // "Male", "Female", "Other"
    public string PreferredLanguage { get; set; } = "English"; // or "Tamil"
    public string Phone { get; set; } = string.Empty;  // Display only,For OTP Verification
    public bool IsWhatsAppVerified { get; set; } = false;
    public int Age { get; set; }

    public DateTime? JoinDate { get; set; }  // Set on approval
    public bool IsApproved { get; set; } = false;

    public string AadhaarFilePath { get; set; } = string.Empty;
    public string PhotoFilePath { get; set; } = string.Empty;

    public DateTime EnrollmentDate { get; set; } = DateTime.Now;
}
