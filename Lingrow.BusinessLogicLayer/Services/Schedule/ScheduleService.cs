using Lingrow.BusinessLogicLayer.DTOs.Schedule;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.BusinessLogicLayer.Mapping;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Enum;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Services.Schedules;

public class ScheduleService : IScheduleService
{
    private readonly IScheduleRepo _scheduleRepo;
    private readonly IStudentCardRepo _cardRepo;

    public ScheduleService(IScheduleRepo scheduleRepo, IStudentCardRepo cardRepo)
    {
        _scheduleRepo = scheduleRepo;
        _cardRepo = cardRepo;
    }

    public async Task<ScheduleResponse> UnpinSeriesAsync(Guid tutorId, Guid scheduleId)
    {
        LoggerHelper.Info($"UnpinSeriesAsync | tutorId={tutorId} | scheduleId={scheduleId}");

        var schedule =
            await _scheduleRepo.GetByIdAsync(scheduleId)
            ?? throw new KeyNotFoundException("Schedule not found.");

        if (schedule.TutorId != tutorId)
            throw new InvalidOperationException("Unauthorized unpin attempt.");

        // 1) Bỏ ghim chính buổi này
        schedule.IsPinned = false;
        await _scheduleRepo.UpdateAsync(schedule);

        // 2) Xoá tất cả buổi pinned cùng chuỗi (3 tháng sau đó)
        await _scheduleRepo.DeletePinnedSeriesAsync(
            tutorId,
            schedule.StudentCardId,
            schedule.StartTime, // đã là UTC vì bạn lưu timestamptz
            months: 3
        );

        LoggerHelper.Info($"Unpin series success | baseScheduleId={schedule.Id}");

        return schedule.ToResponse();
    }

    public async Task<List<ScheduleResponse>> GetWeekAsync(Guid tutorId, DateTime weekStart)
    {
        // Chuẩn hoá weekStart về UTC
        DateTime NormalizeToUtc(DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Utc)
                return dt;

            if (dt.Kind == DateTimeKind.Local)
                return dt.ToUniversalTime();

            // Unspecified -> coi như UTC (vì FE gửi ISO 8601 có "Z")
            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }

        var startUtc = NormalizeToUtc(weekStart);
        var weekEndUtc = startUtc.AddDays(7);

        LoggerHelper.Info(
            $"GetWeekAsync | tutorId={tutorId}, weekStart={startUtc:o}, weekEnd={weekEndUtc:o}"
        );

        var list = await _scheduleRepo.GetByTutorInRangeAsync(tutorId, startUtc, weekEndUtc);

        return list.Select(s => s.ToResponse()).ToList();
    }

    public async Task<ScheduleResponse> CreateScheduleAsync(
        Guid tutorId,
        CreateScheduleRequest request
    )
    {
        LoggerHelper.Info(
            $"CreateScheduleAsync called | tutorId={tutorId} | cardId={request.StudentCardId} | start={request.StartTime} | end={request.EndTime}"
        );

        try
        {
            ScheduleValidation.EnsureValidTimeRange(request.StartTime, request.EndTime);

            var card =
                await _cardRepo.GetByIdAsync(request.StudentCardId)
                ?? throw new KeyNotFoundException("StudentCard not found.");

            if (card.TutorId != tutorId)
                throw new InvalidOperationException(
                    "You do not have permission to schedule this student card."
                );

            var overlap = await _scheduleRepo.HasOverlapAsync(
                tutorId,
                request.StartTime,
                request.EndTime
            );

            if (overlap)
                throw new InvalidOperationException("Schedule overlaps with an existing session.");

            var schedule = new Schedule
            {
                TutorId = tutorId,
                StudentCardId = card.Id,
                Title = request.Title.Trim(),
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Type = request.Type,
                Status = ScheduleStatus.Scheduled,
                IsPinned = request.IsPinned,
            };

            await _scheduleRepo.AddAsync(schedule);

            LoggerHelper.Info($"Create schedule success | scheduleId={schedule.Id}");

            return schedule.ToResponse();
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex);
            throw;
        }
    }

    public async Task<ScheduleResponse> UpdateScheduleAsync(
        Guid tutorId,
        Guid id,
        UpdateScheduleRequest request
    )
    {
        LoggerHelper.Info($"UpdateScheduleAsync called | tutorId={tutorId} | scheduleId={id}");

        try
        {
            ScheduleValidation.EnsureValidTimeRange(request.StartTime, request.EndTime);

            var schedule =
                await _scheduleRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Schedule not found.");

            if (schedule.TutorId != tutorId)
                throw new InvalidOperationException("Unauthorized update attempt.");

            schedule.Title = request.Title.Trim();
            schedule.StartTime = request.StartTime;
            schedule.EndTime = request.EndTime;
            schedule.Type = request.Type;

            if (request.IsPinned.HasValue)
            {
                schedule.IsPinned = request.IsPinned.Value;
            }

            await _scheduleRepo.UpdateAsync(schedule);

            LoggerHelper.Info($"Update schedule success | scheduleId={schedule.Id}");

            return schedule.ToResponse();
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex);
            throw;
        }
    }

    public async Task<bool> DeleteScheduleAsync(Guid tutorId, Guid id)
    {
        LoggerHelper.Info($"DeleteScheduleAsync called | tutorId={tutorId} | scheduleId={id}");

        try
        {
            var schedule =
                await _scheduleRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("Schedule not found.");

            if (schedule.TutorId != tutorId)
                throw new InvalidOperationException("Unauthorized delete attempt.");

            await _scheduleRepo.DeleteAsync(schedule);

            LoggerHelper.Info($"Delete schedule success | scheduleId={id}");

            return true;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex);
            throw;
        }
    }
}
