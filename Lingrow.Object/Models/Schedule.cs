using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lingrow.Enum;

namespace Lingrow.Models;

[Table("schedules")]
public class Schedule : BaseEntity
{
    [Required]
    [Column("tutor_id")]
    public Guid TutorId { get; set; }

    [Required]
    [Column("student_card_id")]
    public Guid StudentCardId { get; set; }

    [Required]
    [MaxLength(200)]
    [Column("title")]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("start_time", TypeName = "timestamptz")]
    public DateTime StartTime { get; set; }

    [Required]
    [Column("end_time", TypeName = "timestamptz")]
    public DateTime EndTime { get; set; }

    [Required]
    [Column("type")]
    public ScheduleType Type { get; set; } = ScheduleType.OneTime;

    [Required]
    [Column("status")]
    public ScheduleStatus Status { get; set; } = ScheduleStatus.Scheduled;

    [ForeignKey("TutorId")]
    public virtual UserAccount? Tutor { get; set; }

    [ForeignKey("StudentCardId")]
    public virtual StudentCard? StudentCard { get; set; }
}
