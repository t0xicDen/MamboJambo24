using Microsoft.EntityFrameworkCore;
using StateManagement.Database.DataAccessLayer.Contracts;
using StateManagement.Database.DataAccessLayer.Data;
using System.Text.Json;

namespace StateManagement.Database.Models;

public class ProcessStateDao<TProcessData> : ProcessState
    where TProcessData : class
{
    private readonly OrchestrationDbContext _orchestrationDbContext;

    internal ProcessStateDao(ProcessState processState, OrchestrationDbContext orchestrationDbContext)
    {
        _orchestrationDbContext = orchestrationDbContext ?? throw new ArgumentNullException(nameof(orchestrationDbContext));

        ArgumentNullException.ThrowIfNull(processState);

        _orchestrationDbContext.Entry(processState).State = EntityState.Detached;
        _orchestrationDbContext.Entry(processState.CreateRequest).State = EntityState.Detached;
        _orchestrationDbContext.Attach(this as ProcessState);
        processState.CreateRequest = null!;

        ProcessStateId = processState.ProcessStateId;
        ProcessTypeId = processState.ProcessTypeId;
        IsLocked = processState.IsLocked;
        LockTimeoutInSeconds = processState.LockTimeoutInSeconds;
        CreateDateTime = processState.CreateDateTime;
        LastModifiedDateTime = processState.LastModifiedDateTime;
        RowVersionStamp = processState.RowVersionStamp;
        CreateRequestId = processState.CreateRequestId;
        ProcessData = processState.ProcessData;
    }

    private TProcessData? _processData;

    public Task UpdateAsync(bool isUnlock)
    {
        _orchestrationDbContext.Entry(this).State = EntityState.Modified;
        
        ProcessData = JsonSerializer.Serialize(DeserializedProcessData);

        IsLocked = isUnlock ? false : true;

        LastModifiedDateTime = DateTime.UtcNow;

        return _orchestrationDbContext.SaveChangesAsync();
    }

    public TProcessData? DeserializedProcessData
    {
        get
        {
            if (_processData == null)
            {
                _processData = GetProcessData();
            }

            return _processData;
        }
        set
        {
            ProcessData = value == null ? null : JsonSerializer.Serialize(value);
            _processData = null;
        }
    }

    private TProcessData? GetProcessData()
    {
        if (ProcessData is null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<TProcessData>(ProcessData);
    }
}