using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lingrow.Enum;

namespace Lingrow.Models;

[Table("user_account")]
public class UserAccount
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; } = Guid.NewGuid();

    // Cognito user identifier ("sub")
    [Required, MaxLength(64)]
    [Column("cognito_sub")]
    public string CognitoSub { get; set; } = default!;

    // Email xác thực bởi Cognito
    [Required, MaxLength(200)]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; } = default!;

    // Username tuỳ chọn (non-nullable)
    [Required, MaxLength(100)]
    [Column("username")]
    public string Username { get; set; } = default!;

    [Required]
    [Column("role")]
    public Role Role { get; set; } = Role.user;

    // Thông tin cá nhân
    [MaxLength(255)]
    [Column("full_name")]
    public string? FullName { get; set; }

    [Column("gender", TypeName = "char(1)")]
    public char? Gender { get; set; }

    [Column("date_of_birth", TypeName = "date")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    // Trạng thái tài khoản
    [Required]
    [Column("status")]
    public UserStatus Status { get; set; } = UserStatus.Active;

    [Column("email_confirmed")]
    public bool EmailConfirmed { get; set; } = false;

    [Column("email_confirmed_at", TypeName = "timestamptz")]
    public DateTime? EmailConfirmedAt { get; set; }

    // Metadata
    [Required]
    [Column("created_at", TypeName = "timestamptz")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at", TypeName = "timestamptz")]
    public DateTime? UpdatedAt { get; set; }

    [Column("last_login_at", TypeName = "timestamptz")]
    public DateTime? LastLoginAt { get; set; }

    [Column("deleted_at", TypeName = "timestamptz")]
    public DateTime? DeletedAt { get; set; }

    // Navigation: luôn nullable hoặc khởi tạo list
    public virtual ICollection<StudentCard>? StudentCardsAsStudent { get; set; }
    public virtual ICollection<StudentCard>? StudentCardsAsTutor { get; set; }
    public virtual ICollection<Schedule>? SchedulesAsTutor { get; set; }
}
