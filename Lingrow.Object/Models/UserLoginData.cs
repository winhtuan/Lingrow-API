using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plantpedia.Enum;

namespace Plantpedia.Models;

[Table("user_login_data")]
public class UserLoginData
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }
    public UserAccount User { get; set; } = default!;

    [Required, MaxLength(100)]
    [Column("username")]
    public string Username { get; set; } = default!;

    [Required, MaxLength(200)]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; } = default!;

    [Required]
    [Column("role")]
    public Role Role { get; set; }

    [Required, Column("password_salt")]
    public byte[] PasswordSalt { get; set; } = default!;

    [Required, Column("password_hash")]
    public byte[] PasswordHash { get; set; } = default!;

    [Required, Column("created_at", TypeName = "timestamptz")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login_at", TypeName = "timestamptz")]
    public DateTime? LastLoginAt { get; set; }

    // Các trường phục vụ auth hiện đại
    [Required, Column("email_confirmed")]
    public bool EmailConfirmed { get; set; } = false;

    [Column("email_confirmed_at", TypeName = "timestamptz")]
    public DateTime? EmailConfirmedAt { get; set; }

    [Required, Column("access_failed_count")]
    public int AccessFailedCount { get; set; } = 0;

    [Column("lockout_end", TypeName = "timestamptz")]
    public DateTime? LockoutEnd { get; set; }

    [Required, Column("two_factor_enabled")]
    public bool TwoFactorEnabled { get; set; } = false;
}
