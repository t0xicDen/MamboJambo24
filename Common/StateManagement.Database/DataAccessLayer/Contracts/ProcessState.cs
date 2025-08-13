using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StateManagement.Database.DataAccessLayer.Contracts;

/// <summary>
/// Entity representing the SessionState table
/// </summary>
[Table("ProcessStates", Schema = "Orchestration")]
public class ProcessState
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ProcessStateId { get; set; }

    [Required]
    public short ProcessTypeId { get; set; }

    [Required]
    public bool IsLocked { get; set; }

    [Required]
    public short LockTimeoutInSeconds { get; set; }

    [Required]
    public DateTime CreateDateTime { get; set; }

    [Required]
    public DateTime LastModifiedDateTime { get; set; }

    [Timestamp]
    public byte[] RowVersionStamp { get; set; } = [];

    [Required]
    [ForeignKey(nameof(CreateRequest))]
    public long CreateRequestId { get; set; }

    public string? ProcessData { get; set; }

    // Navigation property
    public virtual ProcessRequest CreateRequest { get; set; } = null!;
}