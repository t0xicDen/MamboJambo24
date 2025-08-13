using System.ComponentModel.DataAnnotations;

namespace MamboBank.Gateway.WebApi.Models;

public class DepositCallbackRequest
{
    [Required(ErrorMessage = "ExternalTransactionId is required")]
    [StringLength(100, ErrorMessage = "ExternalTransactionId cannot exceed 100 characters")]
    public string ExternalTransactionId { get; set; } = string.Empty;

    [Required(ErrorMessage = "UserId is required")]
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be greater than 0")]
    public int UserId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Status is required")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    [RegularExpression("^(success|completed|failed|rejected|pending)$",
        ErrorMessage = "Status must be one of: success, completed, failed, rejected, pending",
        MatchTimeoutInMilliseconds = 1000)]
    public string Status { get; set; } = string.Empty;
}

public class DepositCallbackResponse
{
    public string Message { get; set; } = string.Empty;
}

public class ExtendedDepositCallbackResponse
{
    public string Message { get; set; } = string.Empty;

    public DateTime TransactionDate { get; set; }
}