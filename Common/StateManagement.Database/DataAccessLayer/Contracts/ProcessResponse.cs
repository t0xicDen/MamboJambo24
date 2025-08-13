using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StateManagement.Database.DataAccessLayer.Contracts;

[Table("ProcessResponses", Schema = "Orchestration")]
public class ProcessResponse
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ResponseId { get; set; }

    [Required]
    [ForeignKey(nameof(ProcessRequest))]
    public required long RequestId { get; set; }

    [Required]
    public required string ResponseBody { get; set; }

    public string? RequestBody { get; set; }

    [Required]
    public DateTime CreateDateTime { get; set; }

    // Navigation property
    public virtual ProcessRequest ProcessRequest { get; set; } = null!;
}
