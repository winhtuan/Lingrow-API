using Lingrow.Models;

namespace Lingrow.DataAccessLayer.Interface;

public interface IScheduleRepo
{
    Task<Schedule?> GetByIdAsync(Guid id);
    Task AddAsync(Schedule schedule);
    Task UpdateAsync(Schedule schedule);
    Task DeleteAsync(Schedule schedule);

    Task<bool> HasOverlapAsync(Guid tutorId, DateTime start, DateTime end);
    Task<List<Schedule>> GetByTutorInRangeAsync(Guid tutorId, DateTime from, DateTime to);
    Task<int> DeletePinnedSeriesAsync(
        Guid tutorId,
        Guid studentCardId,
        DateTime baseStartTimeUtc,
        int months
    );
}
