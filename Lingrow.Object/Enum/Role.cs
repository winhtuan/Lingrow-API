using System.ComponentModel.DataAnnotations;

namespace Lingrow.Enum;

public enum Role
{
    [Display(Name = "Học sinh")]
    student,

    [Display(Name = "Gia sư")]
    tutor,

    [Display(Name = "Giáo viên")]
    teacher,

    [Display(Name = "Quản trị viên")]
    admin,
}
