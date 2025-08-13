using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MamboBank.Gateway.WebApi.Data;
using MamboBank.Gateway.WebApi.Models;

namespace MamboBank.Gateway.WebApi.Controllers;

[ApiController]
public class DepositCallbackController : ControllerBase
{
    private readonly MamboBankDbContext _context;

    public DepositCallbackController(MamboBankDbContext context)
    {
        _context = context;
    }

    [HttpPost("callback")]
    public async Task<IActionResult> DepositCallback([FromBody] DepositCallbackRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // INSERT PAYMENT TRANSACTION 
            Transaction transaction = BuildTransaction(request);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return BuildSuccessResult(request);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {           
            return BuildSuccessResult(request);
        }
    }

    private static Transaction BuildTransaction(DepositCallbackRequest request)
    {
        return new Transaction
        {
            UserId = request.UserId,
            Amount = request.Amount,
            TransactionType = "DEPOSIT",
            TransactionDate = DateTime.UtcNow,
            ExternalTransactionId = request.ExternalTransactionId
        };
    }

    private IActionResult BuildSuccessResult(DepositCallbackRequest request)
    {
        return Ok(new DepositCallbackResponse
        {
            Message = "OK",
        });
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // Check if the exception is related to unique constraint violation
        // For SQL Server, error number 2627 is for unique constraint violation
        return ex.InnerException?.Message?.Contains("unique", StringComparison.OrdinalIgnoreCase) == true ||
               ex.InnerException?.Message?.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
               ex.InnerException?.Message?.Contains("2627") == true;
    }
}