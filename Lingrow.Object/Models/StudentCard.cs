using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lingrow.Enum;

namespace Lingrow.Models;

[Table("student_cards")]
public class StudentCard : BaseEntity
{
    [Required]
    [Column("tutor_id")]
    public Guid TutorId { get; set; }

    [Required]
    [Column("student_id")]
    public Guid StudentId { get; set; }

    // Bắt buộc => non-nullable + khởi tạo mặc định
    [Required]
    [MaxLength(200)]
    [Column("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    // Không bắt buộc => string? để tránh warning
    [MaxLength(1000)]
    [Column("notes")]
    public string? Notes { get; set; }

    // JSON => optional
    [Column("tags", TypeName = "jsonb")]
    public string? Tags { get; set; }

    // Bắt buộc => non-nullable + default
    [Required]
    [MaxLength(50)]
    [Column("color")]
    public string Color { get; set; } = "blue";

    // Navigation
    [ForeignKey("TutorId")]
    public virtual UserAccount? Tutor { get; set; }

    [ForeignKey("StudentId")]
    public virtual UserAccount? Student { get; set; }

    public virtual ICollection<Schedule>? Schedules { get; set; }
}
