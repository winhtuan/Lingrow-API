using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Plantpedia.Enum;
using Plantpedia.Models;

[Table("user_activity")]
public class UserActivity
{
    [Key]
    [Column("activity_id")]
    public long ActivityId { get; set; }

    [Required, Column("user_id")]
    public long UserId { get; set; }
    public UserAccount User { get; set; } = default!;

    [Required, Column("type")]
    public ActivityType Type { get; set; }

    [MaxLength(100)]
    [Column("ref_id")]
    public string? RefId { get; set; }

    [MaxLength(50)]
    [Column("ref_type")]
    public string? RefType { get; set; }

    [Column("metadata", TypeName = "jsonb")]
    public JsonDocument? Metadata { get; set; }

    [Required, Column("created_at", TypeName = "timestamptz")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(45)]
    [Column("ip")]
    public string? Ip { get; set; }

    [MaxLength(256)]
    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [MaxLength(64)]
    [Column("correlation_id")]
    public string? CorrelationId { get; set; }
}
