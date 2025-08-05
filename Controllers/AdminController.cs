using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MipWeb.Data;
using MipWeb.Models;

namespace MipWeb.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly MipWebContext _context;

    private async Task<int> numberOfStudentsInYear(int year)
    {
        return await _context.Students.CountAsync(s =>
            s.IsApproved && s.JoinDate != null && s.JoinDate.Value.Year == year);
    }

    private async Task<Student> findStudentById(int id)
    {
        return await _context.Students.FindAsync(id);
    }

    public AdminController(MipWebContext context) => _context = context;

    public async Task<IActionResult> PendingApproval()
    {
        var students = await _context.Students
            .Where(s => !s.IsApproved)
            .ToListAsync();
        return View(students);
    }

    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> ApproveStudent(int id)
    {
        var student = await findStudentById(id);
        if (student == null) return NotFound();

        student.IsApproved = true;
        student.JoinDate = DateTime.Now;

        // Student ID logic
        var year = student.JoinDate.Value.Year % 100;
        int count = await numberOfStudentsInYear(student.JoinDate.Value.Year);
        student.StudentId = $"MIP{year:D2}{count + 1:D3}";

        await _context.SaveChangesAsync();

        // TODO: Trigger WhatsApp API to send "Approval" notification
        return RedirectToAction("PendingApproval");
    }

    [HttpPost]
    public async Task<IActionResult> RejectStudent(int id, string reason)
    {
        var student = await findStudentById(id);
        if (student == null) return NotFound();

        // TODO: Trigger WhatsApp API to send rejection with reason
        // await _whatsAppService.SendRejection(student.PhoneNumber, reason);

        // Delete record after notification
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return RedirectToAction("PendingApproval");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateJoinDate(int id, DateTime joinDate)
    {
        var student = await findStudentById(id);
        if (student == null) return NotFound();

        student.JoinDate = joinDate;

        // If StudentID is null, assign based on the joinDate
        if (string.IsNullOrEmpty(student.StudentId))
        {
            var year = joinDate.Year % 100;
            var count = await numberOfStudentsInYear(student.JoinDate.Value.Year);
            student.StudentId = $"MIP{year:D2}{count + 1:D3}";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("PendingApproval"); // or ApprovedStudents
    }

}
