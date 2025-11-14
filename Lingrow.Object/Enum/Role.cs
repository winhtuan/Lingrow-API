using System.ComponentModel.DataAnnotations;

namespace Lingrow.Enum;

public enum Role
{
    [Display(Name = "Người dùng")]
    user,

    [Display(Name = "Giáo Viên")]
    teacher,

    [Display(Name = "Quản trị viên")]
    admin,
}
