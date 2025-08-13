namespace MamboBank.Gateway.WebApi.Models;

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty; // Credit, Debit
    public DateTime TransactionDate { get; set; }
    public string ExternalTransactionId { get; set; } = string.Empty; // Unique external transaction identifier
}