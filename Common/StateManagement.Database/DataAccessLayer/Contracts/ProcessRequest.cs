using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StateManagement.Database.DataAccessLayer.Contracts;

[Table("ProcessRequests", Schema = "Orchestration")]
public class ProcessRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RequestId { get; set; }

    [Required]
    public required DateTime RequestDateTime { get; set; }

    [Required]
    public required string ExternalRequestId { get; set; }

    // Navigation property - one ProcessStateRequest can have many ProcessStates
    public virtual ICollection<ProcessState> ProcessStates { get; set; } = new List<ProcessState>();

    // Navigation property - one ProcessStateRequest can have many ProcessResponses
    public virtual ICollection<ProcessResponse> ProcessResponses { get; set; } = new List<ProcessResponse>();
}
