using System.ComponentModel.DataAnnotations.Schema;

namespace MipWeb.Models;

public class ExpenseCategory
{
    public int ExpenseCategoryID { get; set; }
    public string CategoryName { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Expense> Expenses { get; set; }
}

public class Expense
{
    public int ExpenseID { get; set; }
    public DateTime ExpenseDate { get; set; }
    public int CategoryID { get; set; }
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public string ModeOfPayment { get; set; }
    public int? PartnerUserID { get; set; } // null = school funds
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [ForeignKey("CategoryID")]
    public ExpenseCategory Category { get; set; }
    [ForeignKey("PartnerUserID")]
    public User? PartnerUser { get; set; }
    [ForeignKey("CreatedBy")]
    public User CreatedByUser { get; set; }
}
