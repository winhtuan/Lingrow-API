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
