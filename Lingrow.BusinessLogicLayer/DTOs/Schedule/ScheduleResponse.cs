using Lingrow.Enum;

namespace Lingrow.BusinessLogicLayer.DTOs.Schedule;

public class ScheduleResponse
{
    public Guid Id { get; set; }
    public Guid TutorId { get; set; }
    public Guid StudentCardId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ScheduleType Type { get; set; }
    public ScheduleStatus Status { get; set; }
    public bool IsPinned { get; set; }
}
