using System.ComponentModel.DataAnnotations;

namespace Plantpedia.Enum;

public enum Role
{
    [Display(Name = "Người dùng")]
    user,

    [Display(Name = "Chuyên gia")]
    expert,

    [Display(Name = "Quản trị viên")]
    admin,
}
