namespace Lingrow.BusinessLogicLayer.Services.Schedules;

public static class ScheduleValidation
{
    public static void EnsureValidTimeRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("EndTime must be greater than StartTime.");
    }
}
