using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MamboBank.Gateway.WebApi.Data;
using MamboBank.Gateway.WebApi.Models;
using StateManagement.Database.Services;
using System.Text.Json;

namespace MamboBank.Gateway.WebApi.Controllers;

[ApiController]
public class FakeOrchestrationController : ControllerBase
{
    private readonly MamboBankDbContext _context;
    private readonly ProcessService _processService;

    private const int ProcessTypeDeposit = 1; // Deposit

    public FakeOrchestrationController(MamboBankDbContext context, ProcessService processService)
    {
        _context = context;
        _processService = processService;
    }

    [HttpPost("callback-4")]
    public async Task<IActionResult> DepositCallback([FromBody] DepositCallbackRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // CREATE PROCESS STATE+REQUEST
            var processState = await _processService.CreateOrGetProcessStateAsync(
                request.ExternalTransactionId,
                ProcessTypeDeposit, 
                30, // Lock timeout in seconds
                new DepositProcessData {  CurrentStep = "CreateTransaction" }, CancellationToken.None);

            var requestId = processState.CreateRequestId;

            // LOCK PROCESS STATE
            if (!await processState.TryLockAsync())
            {
                return BadRequest("In process");
            }

            // START STEP BY STEP FLOW

            if (processState.DeserializedProcessData!.CurrentStep == "CreateTransaction")
            {
                CreateTransaction();

                processState.DeserializedProcessData.CurrentStep = "UpdateBalance";
                await processState.UpdateAsync(isUnlock: false);
            }

            if (processState.DeserializedProcessData!.CurrentStep == "UpdateBalance")
            {
                UpdateBalance();

                processState.DeserializedProcessData.CurrentStep = "ApplyBonus";
                await processState.UpdateAsync(isUnlock: false);
            }

            if (processState.DeserializedProcessData!.CurrentStep == "ApplyBonus")
            {
                ApplyBonus();

                processState.DeserializedProcessData.CurrentStep = "SendEmail";
                await processState.UpdateAsync(isUnlock: false);
            }

            if (processState.DeserializedProcessData!.CurrentStep == "SendEmail")
            {
                SendEmail();

                processState.DeserializedProcessData.CurrentStep = string.Empty;
                await processState.UpdateAsync(isUnlock: true);
            }

            // BUILD & PERSIST PROCESS RESPONSE
            var response = BuildSuccessResponse(request);

            await _processService.CreateProcessResponseAsync(
                processState.CreateRequestId,
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

    private void CreateTransaction() { }
    private void UpdateBalance() { }
    private void ApplyBonus() { }
    private void SendEmail() { }


    private DepositCallbackResponse BuildSuccessResponse(DepositCallbackRequest request)
    {
        return new DepositCallbackResponse
        {
            Message = "OK"
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

public class DepositProcessData
{
    public string CurrentStep { get; set; }
}