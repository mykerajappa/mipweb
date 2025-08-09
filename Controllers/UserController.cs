using Microsoft.AspNetCore.Mvc;
using MipWeb.Models;
using MipWeb.ViewModels;
using MipWeb.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace MipWeb.Controllers
{
    public class UserController : Controller
    {
        private readonly MipWebContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(MipWebContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Main user management page
        public IActionResult Index()
        {
            return View();
        }

        // Partial: list users
        public IActionResult UserListPartial()
        {
            var users = _context.GetUsersWithRoles().ToList();

            return PartialView("_UserListPartial", users);
        }

        // Create user modal (GET)
        public IActionResult Create()
        {
            var model = new UserViewModel
            {
                AllRoles = _context.Roles.Select(r => r.Name).Distinct().ToList()
            };
            return PartialView("_UserModalPartial", model);
        }

        // Edit user modal (GET)
        public IActionResult Edit(int id)
        {
            var user = _context.GetUsersWithRoles().FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            var model = new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                UserId = user.Username,
                Email = user.Email,
                WhatsappNumber = user.WhatsAppNumber,
                SelectedRoles = user.UserRoles.Select(r => r.Role.Name).ToList(),
                AllRoles = _context.Roles.Select(r => r.Name).ToList()
            };

            return PartialView("_UserModalPartial", model);
        }

        // Save user (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(UserViewModel model)
        {
            var RoleObjects = _context.Roles;

            if (!ModelState.IsValid)
            {
                model.AllRoles = RoleObjects.Select(r => r.Name).Distinct().ToList();
                return PartialView("_UserModalPartial", model);
            }

            var selectedUserRoles = RoleObjects
                                    .Where(r => model.SelectedRoles.Contains(r.Name))
                                    .Select(rid => new UserRole { Role = rid })
                                    .ToList();

            if (model.Id == 0)
            {
                // New user
                var user = new User
                {
                    Name = model.Name,
                    Username = model.UserId,
                    Email = model.Email,
                    WhatsAppNumber = model.WhatsappNumber,
                    PasswordHash = Helpers.PasswordHelper.Hash(model.Password),
                    UserRoles = selectedUserRoles
                };

                _context.Users.Add(user);
            }
            else
            {
                // Update existing
                var user = _context.GetUsersWithRoles().FirstOrDefault(u => u.Id == model.Id);

                if (user == null) return NotFound();

                user.Name = model.Name;
                user.Email = model.Email;
                user.WhatsAppNumber = model.WhatsappNumber;

                user.UserRoles = selectedUserRoles;

                _context.Users.Update(user);
            }

            _context.SaveChanges();
            return Json(new { success = true });
        }

        // Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();
            return Ok();
        }

        // Deactivate
        [HttpPost]
        public IActionResult Deactivate(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.IsActive = false;
            _context.SaveChanges();
            return Ok();
        }

        // Reset password
        [HttpPost]
        public IActionResult ResetPassword(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            string newPassword = Helpers.PasswordHelper.GenerateSecurePassword();
            user.PasswordHash = Helpers.PasswordHelper.Hash(newPassword);

            _context.SaveChanges();

            // Send via WhatsApp
            SendWhatsAppPassword(user.WhatsAppNumber, newPassword);

            return Ok();
        }

        private void SendWhatsAppPassword(string phone, string newPassword)
        {
            // You’ll implement actual WhatsApp integration here.
            _logger.LogInformation($"WhatsApp to {phone}: Your new password is {newPassword}");
        }
    }
}
