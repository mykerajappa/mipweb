using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MipWeb.Data;
using MipWeb.Models;

namespace MipWeb.Controllers;

[Authorize(Roles = "Admin")] 
public class ExpenseController : Controller
{
    private readonly MipWebContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExpenseController(MipWebContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    // GET: Create Expense
    public IActionResult Create()
    {
        ViewBag.Categories = _context.ExpenseCategories.Where(c => c.IsActive).ToList();
        ViewBag.Partners = _context.GetUsersWithRoles()
            .Where(u => u.UserRoles.Any(r => r.Role.Name == AppRoles.Partner))
            .ToList();
        return View();
    }

    // GET: Expense List
    public IActionResult Index(int? month, int? year, int? categoryId, int? partnerUserId)
    {
        // Default to current month/year if not provided
        if (!month.HasValue || !year.HasValue)
        {
            month = DateTime.Now.Month;
            year = DateTime.Now.Year;
        }

        var query = _context.Expenses
            .Include(e => e.Category)
            .Include(e => e.PartnerUser)
            .Include(e => e.CreatedByUser)
            .AsQueryable();

        if (month.HasValue && year.HasValue)
            query = query.Where(e => e.ExpenseDate.Month == month && e.ExpenseDate.Year == year);

        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(e => e.CategoryID == categoryId);

        if (partnerUserId.HasValue)
            query = query.Where(e => e.PartnerUser.Id == partnerUserId);

        var expenses = query
            .OrderByDescending(e => e.ExpenseDate)
            .ToList();

        ViewBag.TotalAmount = expenses.Sum(e => e.Amount);
        ViewBag.Categories = _context.ExpenseCategories.Where(c => c.IsActive).ToList();
        ViewBag.Partners = _context.Users
            .Where(u => u.UserRoles.Any(r => r.Role.Name == AppRoles.Partner))
            .ToList();
        ViewBag.SelectedMonth = month;
        ViewBag.SelectedYear = year;

        return View(expenses);
    }


    // POST: Create Expense
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Expense expense)
    {
        if (expense.CategoryID == _context.ExpenseCategories
                .First(c => c.CategoryName == "Other").ExpenseCategoryID
            && string.IsNullOrWhiteSpace(expense.Description))
        {
            ModelState.AddModelError("Description", "Description is required for 'Other' category.");
        }

        ModelState.Remove("Category");
        ModelState.Remove("PartnerUser");
        ModelState.Remove("CreatedByUser");

        if (ModelState.IsValid)
        {
            var currentUserId = int.Parse(_httpContextAccessor.HttpContext.User
                .FindFirst("UserID").Value);

            expense.CreatedBy = currentUserId;
            expense.CreatedAt = DateTime.UtcNow;

            _context.Expenses.Add(expense);
            _context.SaveChanges();
            ViewBag.SuccessMessage = "Expense added successfully.";

            // Keep Date, reset other fields
            var newModel = new Expense
            {
                ExpenseDate = expense.ExpenseDate
            };

            ViewBag.Categories = _context.ExpenseCategories.ToList();
            ViewBag.Partners = _context.GetPartners().ToList();
            return View(newModel);
        }

        ViewBag.Categories = _context.ExpenseCategories.Where(c => c.IsActive).ToList();
        ViewBag.Partners = _context.GetPartners().ToList();
        return View(expense);
    }

    // POST: Add Category via AJAX
    [HttpPost]
    public IActionResult AddCategory(string categoryName)
    {
        if (!string.IsNullOrWhiteSpace(categoryName))
        {
            var category = new ExpenseCategory { CategoryName = categoryName, IsActive = true };
            _context.ExpenseCategories.Add(category);
            _context.SaveChanges();
            return Json(new { success = true, categoryId = category.ExpenseCategoryID, categoryName });
        }
        return Json(new { success = false, message = "Invalid category name" });
    }
}
