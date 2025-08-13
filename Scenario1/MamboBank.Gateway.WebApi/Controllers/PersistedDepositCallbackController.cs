using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MamboBank.Gateway.WebApi.Data;
using MamboBank.Gateway.WebApi.Models;
using StateManagement.Database.Services;
using System.Text.Json;

namespace MamboBank.Gateway.WebApi.Controllers;

[ApiController]
public class PersistedDepositCallbackController : ControllerBase
{
    private readonly MamboBankDbContext _context;
    private readonly ProcessService _processService;

    public PersistedDepositCallbackController(MamboBankDbContext context, ProcessService processService)
    {
        _context = context;
        _processService = processService;
    }

    [HttpPost("callback-3")]
    public async Task<IActionResult> DepositCallback([FromBody] DepositCallbackRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // CREATE PROCESS REQUEST
            var processRequest = await _processService.CreateProcessStateRequestAsync(request.ExternalTransactionId, CancellationToken.None);

            // INSERT PAYMENT TRANSACTION 
            Transaction transaction = BuildTransaction(request);
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // BUILD & PERSIST PROCESS RESPONSE
            var response = BuildSuccessResponse(request, transaction);

            await _processService.CreateProcessResponseAsync(
                processRequest.RequestId,
                JsonSerializer.Serialize(response),
                JsonSerializer.Serialize(request),
                CancellationToken.None);

            return Ok(response);
        }
        catch (ExternalRequestIdAlreadyExistsException ex)
        {
            var processResponse = await _processService.GetProcessResponseByExternalRequestIdAsync(ex.ExternalRequestId, CancellationToken.None);

            var deserializedResponse = JsonSerializer.Deserialize<ExtendedDepositCallbackResponse>(processResponse.ResponseBody);

            return Ok(deserializedResponse);
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

    private ExtendedDepositCallbackResponse BuildSuccessResponse(DepositCallbackRequest request, Transaction transaction)
    {
        return new ExtendedDepositCallbackResponse
        {
            Message = "OK",
            TransactionDate = transaction.TransactionDate
        };
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