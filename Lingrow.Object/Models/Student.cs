using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lingrow.Enum;

namespace Lingrow.Models;

public class Student : UserAccount
{
    public Student()
    {
        // Dùng Role làm "discriminator" logic trong code
        Role = Role.student;
    }

    [Column("age")]
    public int? Age { get; set; }

    [MaxLength(100)]
    [Column("level")]
    public string? Level { get; set; } // Ví dụ: "Beginner"

    [MaxLength(500)]
    [Column("goals")]
    public string? Goals { get; set; } // Ví dụ: "Improve vocabulary"

    [MaxLength(100)]
    [Column("preferred_language")]
    public string? PreferredLanguage { get; set; } // SY-01: đa ngôn ngữ
}
