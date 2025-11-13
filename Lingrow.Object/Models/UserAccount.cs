using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Plantpedia.Enum;

namespace Plantpedia.Models;

[Table("user_account")]
public class UserAccount
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Required, MaxLength(255)]
    [Column("full_name")]
    public string FullName { get; set; } = default!;

    [Required, Column("gender", TypeName = "char(1)")]
    public char Gender { get; set; }

    [Column("date_of_birth", TypeName = "date")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("avatar_url")]
    public string? AvatarUrl { get; set; }

    [Required, Column("status")]
    public UserStatus Status { get; set; } = UserStatus.Active;

    [Required, Column("created_at", TypeName = "timestamptz")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at", TypeName = "timestamptz")]
    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at", TypeName = "timestamptz")]
    public DateTime? DeletedAt { get; set; }

    public UserLoginData LoginData { get; set; } = default!;
}
