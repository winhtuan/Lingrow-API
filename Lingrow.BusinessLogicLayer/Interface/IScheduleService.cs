using Lingrow.BusinessLogicLayer.DTOs.Schedule;

namespace Lingrow.BusinessLogicLayer.Interface;

public interface IScheduleService
{
    Task<ScheduleResponse> CreateScheduleAsync(Guid tutorId, CreateScheduleRequest request);
    Task<ScheduleResponse> UpdateScheduleAsync(
        Guid tutorId,
        Guid scheduleId,
        UpdateScheduleRequest request
    );
    Task<bool> DeleteScheduleAsync(Guid tutorId, Guid scheduleId);
    Task<List<ScheduleResponse>> GetWeekAsync(Guid tutorId, DateTime weekStart);
    Task<ScheduleResponse> UnpinSeriesAsync(Guid tutorId, Guid scheduleId);
}
