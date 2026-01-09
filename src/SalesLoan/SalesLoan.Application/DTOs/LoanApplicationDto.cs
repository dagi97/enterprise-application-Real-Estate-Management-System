namespace SalesLoan.Application.DTOs;

public class LoanApplicationDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public Guid BranchId { get; set; }
    public string CustomerFirstName { get; set; } = string.Empty;
    public string CustomerLastName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public decimal? ApprovedAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTime AppliedAt { get; set; }
    public bool IsPropertyReserved { get; set; }
}

