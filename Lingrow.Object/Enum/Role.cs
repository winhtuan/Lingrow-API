using System.ComponentModel.DataAnnotations;

namespace Lingrow.Enum;

public enum Role
{
    [Display(Name = "Học sinh")]
    user,

    [Display(Name = "Gia sư")]
    tutor,

    [Display(Name = "Giáo viên")]
    teacher,

    [Display(Name = "Quản trị viên")]
    admin,
}
