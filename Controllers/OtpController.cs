using Microsoft.AspNetCore.Mvc;
using MipWeb.Data;
using MipWeb.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using MipWeb.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MipWeb.Controllers
{
    public class OtpRequestModel
    {
        public string PhoneNumber { get; set; }
    }

    [ApiController]
    [Route("Otp")]
    public class OtpController : Controller
    {
        private readonly MipWebContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly WhatsAppService _whatsAppService;

        public OtpController(MipWebContext context, IHttpClientFactory httpClientFactory, WhatsAppService whatsappSvc)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _whatsAppService = whatsappSvc;
        }

        [HttpGet("Ping")]
        public IActionResult Ping() => Ok("OtpController is reachable");

        [HttpPost("SendOtp")]
        public async Task<IActionResult> SendOtp([FromBody] OtpRequestModel model)
        {
            var existing = await _context.OtpVerifications
                .Where(o => o.PhoneNumber == model.PhoneNumber)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (existing != null && existing.CreatedAt.AddMinutes(1) > DateTime.UtcNow)
            {
                return BadRequest(new { message = "OTP already sent. Please wait before requesting again." });
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                return BadRequest(new { message = "Phone number is required." });
            }

            try
            {
                var newOtp = new OtpVerification
                {
                    PhoneNumber = model.PhoneNumber,
                    OtpCode = new Random().Next(100000, 999999).ToString(), // your OTP logic
                    IsVerified = false,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5)
                };

                _context.OtpVerifications.Add(newOtp);
                await _context.SaveChangesAsync();

                await _whatsAppService.SendOtp(model.PhoneNumber, newOtp.OtpCode);

                return Ok(new { message = $"OTP sent to {model.PhoneNumber}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Server error: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult VerifyOtp(string phoneNumber, string otpCode)
        {
            var now = DateTime.UtcNow;

            var record = _context.OtpVerifications
                .Where(x => x.PhoneNumber == phoneNumber && !x.IsVerified && x.ExpiresAt > now)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (record == null || record.OtpCode != otpCode)
            {
                return Json(new { success = false, message = "Invalid or expired OTP." });
            }

            record.IsVerified = true;
            _context.SaveChanges();

            return Json(new { success = true, message = "OTP verified successfully." });
        }
    }
}
