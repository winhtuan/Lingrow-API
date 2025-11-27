using Lingrow.DataAccessLayer.Data;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Enum;
using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.DataAccessLayer.Repository;

public class ScheduleRepo : IScheduleRepo
{
    private readonly AppDbContext _context;

    public ScheduleRepo(AppDbContext db)
    {
        _context = db;
    }

    public async Task<int> DeletePinnedSeriesAsync(
        Guid tutorId,
        Guid studentCardId,
        DateTime baseStartTimeUtc,
        int months
    )
    {
        // baseStartTimeUtc là StartTime của buổi mà bạn đang bỏ ghim (UTC)
        var fromUtc = baseStartTimeUtc.AddDays(1); // sau buổi hiện tại
        var toUtc = baseStartTimeUtc.AddMonths(months); // tới N tháng sau

        var timeOfDay = baseStartTimeUtc.TimeOfDay; // 08:00, 09:00,...
        return await _context
            .Schedules.Where(s =>
                s.TutorId == tutorId
                && s.StudentCardId == studentCardId
                && s.IsPinned
                && !s.IsDeleted
                && s.StartTime >= fromUtc
                && s.StartTime <= toUtc
                && s.StartTime.TimeOfDay == timeOfDay
            )
            .ExecuteDeleteAsync();
    }

    public async Task<List<Schedule>> GetByTutorInRangeAsync(
        Guid tutorId,
        DateTime from,
        DateTime to
    )
    {
        DateTime NormalizeToUtc(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Utc)
                return dt;

            if (dt.Kind == DateTimeKind.Unspecified)
            {
                var asUtc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                return asUtc;
            }

            return dt.ToUniversalTime();
        }

        var fromUtc = NormalizeToUtc(from);
        var toUtc = NormalizeToUtc(to);

        return await _context
            .Schedules.Include(s => s.StudentCard)
            .Where(s =>
                s.TutorId == tutorId
                && !s.IsDeleted
                && s.StartTime >= fromUtc
                && s.StartTime < toUtc
            )
            .ToListAsync();
    }

    public Task<Schedule?> GetByIdAsync(Guid id) =>
        _context.Schedules.FirstOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(Schedule schedule)
    {
        _context.Schedules.Add(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Schedule schedule)
    {
        _context.Schedules.Update(schedule);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Schedule schedule)
    {
        _context.Schedules.Remove(schedule);
        await _context.SaveChangesAsync();
    }

    public Task<bool> HasOverlapAsync(Guid tutorId, DateTime start, DateTime end)
    {
        return _context.Schedules.AnyAsync(s =>
            s.TutorId == tutorId
            && s.Status == ScheduleStatus.Scheduled
            && s.StartTime < end
            && s.EndTime > start
        );
    }
}
