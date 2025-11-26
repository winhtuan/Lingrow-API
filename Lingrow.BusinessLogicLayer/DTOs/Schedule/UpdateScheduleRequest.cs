using System.ComponentModel.DataAnnotations;
using Lingrow.Enum;

namespace Lingrow.BusinessLogicLayer.DTOs.Schedule;

public class UpdateScheduleRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public ScheduleType Type { get; set; } = ScheduleType.OneTime;
}
