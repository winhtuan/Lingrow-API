using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lingrow.Enum;

namespace Lingrow.Models;

public class Tutor : UserAccount
{
    public Tutor()
    {
        Role = Role.tutor;
    }

    // Thuộc tính riêng cho Tutor
    [MaxLength(500)]
    [Column("bio")]
    public string? Bio { get; set; }

    [MaxLength(255)]
    [Column("expertise")] // Ví dụ: "English, Math"
    public string? Expertise { get; set; }

    [MaxLength(100)]
    [Column("languages")] // Ví dụ: "English, Vietnamese"
    public string? Languages { get; set; }

    [Column("hourly_rate")]
    public decimal HourlyRate { get; set; } = 0m;
}
