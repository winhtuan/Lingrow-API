using Lingrow.BusinessLogicLayer.DTOs.Schedule;
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

    public async Task<ScheduleResponse> CreateScheduleAsync(
        Guid tutorId,
        CreateScheduleRequest request
    )
    {
        // Validate thời gian
        ScheduleValidation.EnsureValidTimeRange(request.StartTime, request.EndTime);

        // Lấy student card
        var card =
            await _cardRepo.GetByIdAsync(request.StudentCardId)
            ?? throw new KeyNotFoundException("StudentCard not found.");

        // Check quyền tutor
        if (card.TutorId != tutorId)
            throw new InvalidOperationException(
                "You do not have permission to schedule this student card."
            );

        // Check overlap (trùng giờ)
        var overlap = await _scheduleRepo.HasOverlapAsync(
            tutorId,
            request.StartTime,
            request.EndTime
        );

        if (overlap)
            throw new InvalidOperationException("Schedule overlaps with an existing session.");

        // Create entity
        var schedule = new Schedule
        {
            TutorId = tutorId,
            StudentCardId = card.Id,
            Title = request.Title.Trim(),
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Type = request.Type,
            Status = ScheduleStatus.Scheduled,
        };

        await _scheduleRepo.AddAsync(schedule);

        return schedule.ToResponse();
    }

    public async Task<ScheduleResponse> UpdateScheduleAsync(
        Guid tutorId,
        Guid id,
        UpdateScheduleRequest request
    )
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

        await _scheduleRepo.UpdateAsync(schedule);

        return schedule.ToResponse();
    }

    public async Task<bool> DeleteScheduleAsync(Guid tutorId, Guid id)
    {
        var schedule =
            await _scheduleRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Schedule not found.");

        if (schedule.TutorId != tutorId)
            throw new InvalidOperationException("Unauthorized delete attempt.");

        await _scheduleRepo.DeleteAsync(schedule);

        return true;
    }
}
