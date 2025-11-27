using Lingrow.BusinessLogicLayer.DTOs.Schedule;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Mapping;

public static class ScheduleMapping
{
    public static ScheduleResponse ToResponse(this Schedule s)
    {
        return new ScheduleResponse
        {
            Id = s.Id,
            TutorId = s.TutorId,
            StudentCardId = s.StudentCardId,
            Title = s.Title,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Type = s.Type,
            Status = s.Status,
            IsPinned = s.IsPinned,
        };
    }
}
