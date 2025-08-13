using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StateManagement.Database.DataAccessLayer.Contracts;
using StateManagement.Database.DataAccessLayer.Data;
using StateManagement.Database.Models;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Linq; // Added for query operations

namespace StateManagement.Database.Services;

/// <summary>
/// Exception thrown when attempting to create a ProcessStateRequest with a duplicate ExternalRequestId.
/// </summary>
public sealed class ExternalRequestIdAlreadyExistsException : Exception
{
    public string ExternalRequestId { get; }
    public ExternalRequestIdAlreadyExistsException(string externalRequestId, Exception? inner = null)
        : base($"A ProcessStateRequest with ExternalRequestId '{externalRequestId}' already exists.", inner)
    {
        ExternalRequestId = externalRequestId;
    }
}

/// <summary>
/// Service exposing operations for creating process related entities while enforcing uniqueness of ExternalRequestId.
/// </summary>
public class ProcessService
{
    private readonly OrchestrationDbContext _dbContext;
    private readonly ILogger<ProcessService> _logger;

    public ProcessService(OrchestrationDbContext dbContext, ILogger<ProcessService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves the latest ProcessResponse associated with the given ExternalRequestId.
    /// Returns null if none exists.
    /// </summary>
    public Task<ProcessResponse?> GetProcessResponseByExternalRequestIdAsync(string externalRequestId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving ProcessResponse by ExternalRequestId={ExternalRequestId}", externalRequestId);
        return _dbContext.ProcessResponses
            .Where(r => r.ProcessRequest.ExternalRequestId == externalRequestId)
            .OrderByDescending(r => r.CreateDateTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a ProcessStateRequest and an associated ProcessState, returning the ProcessState wrapped in a DAO.
    /// </summary>
    public async Task<ProcessStateDao<TProcessData>> CreateOrGetProcessStateAsync<TProcessData>(
        string externalRequestId,
        short processTypeId,
        short lockTimeoutInSeconds,
        TProcessData? processData = null,
        CancellationToken cancellationToken = default)
        where TProcessData : class
    {
        _logger.LogInformation("Creating ProcessStateRequest + ProcessState (ExternalRequestId={ExternalRequestId})", externalRequestId);

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        ProcessRequest request = null;

        try
        {
            request = new ProcessRequest
            {
                ExternalRequestId = externalRequestId,
                RequestDateTime = DateTime.UtcNow
            };
            _dbContext.ProcessStateRequests.Add(request);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var processState = new ProcessState
            {
                CreateRequestId = request.RequestId,
                ProcessTypeId = processTypeId,
                IsLocked = false,
                LockTimeoutInSeconds = lockTimeoutInSeconds,
                CreateDateTime = now,
                LastModifiedDateTime = now,
                ProcessData = processData != null ? JsonSerializer.Serialize(processData) : null
            };
            _dbContext.ProcessStates.Add(processState);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Created ProcessState (Id={ProcessStateId}) for Request (Id={RequestId})", processState.ProcessStateId, request.RequestId);
            return new ProcessStateDao<TProcessData>(processState, _dbContext);
        }
        catch (DbUpdateException ex) when (IsExternalRequestIdUniqueViolation(ex))
        {
            await transaction.RollbackAsync(cancellationToken);

            _dbContext.Entry(request).State = EntityState.Detached;

            var processRequest = _dbContext.ProcessStateRequests
                .AsNoTracking()
                .Include(r => r.ProcessStates)
                .First(x => x.ExternalRequestId == externalRequestId);
            
            return new ProcessStateDao<TProcessData>(
                processRequest.ProcessStates.First(), _dbContext);
        }
    }

    /// <summary>
    /// Creates only a ProcessStateRequest.
    /// </summary>
    public async Task<ProcessRequest> CreateProcessStateRequestAsync(string externalRequestId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating ProcessStateRequest (ExternalRequestId={ExternalRequestId})", externalRequestId);
        var request = new ProcessRequest
        {
            ExternalRequestId = externalRequestId,
            RequestDateTime = DateTime.UtcNow
        };

        try
        {
            _dbContext.ProcessStateRequests.Add(request);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created ProcessStateRequest (Id={RequestId})", request.RequestId);
            return request;
        }
        catch (DbUpdateException ex) when (IsExternalRequestIdUniqueViolation(ex))
        {
            _logger.LogWarning(ex, "Duplicate ExternalRequestId detected while creating ProcessStateRequest: {ExternalRequestId}", externalRequestId);
            throw new ExternalRequestIdAlreadyExistsException(externalRequestId, ex);
        }
    }

    /// <summary>
    /// Creates a ProcessStateRequest and a ProcessResponse attached to it.
    /// </summary>
    public async Task<ProcessResponse> CreateProcessResponseAsync(long requestId, string responseBody, string? requestBody = null, CancellationToken cancellationToken = default)
    {
        var response = new ProcessResponse
        {
            RequestId = requestId,
            ResponseBody = responseBody,
            RequestBody = requestBody,
            CreateDateTime = DateTime.UtcNow
        };
        _dbContext.ProcessResponses.Add(response);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }

    private static bool IsExternalRequestIdUniqueViolation(DbUpdateException ex)
    {
        if (ex.InnerException is SqlException sqlEx)
        {
            // 2601: Cannot insert duplicate key row in object with unique index.
            // 2627: Violation of %ls constraint. Cannot insert duplicate key in %ls.
            if (sqlEx.Number is 2601 or 2627 && sqlEx.Message.Contains("ExternalRequestId", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        // Fallback to message inspection if inner exception is not SqlException
        var message = ex.InnerException?.Message ?? ex.Message;
        return message.Contains("ExternalRequestId", StringComparison.OrdinalIgnoreCase)
               && (message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || message.Contains("unique", StringComparison.OrdinalIgnoreCase));
    }
}
