// Lingrow.BusinessLogicLayer/Services/Schedules/StudentCardService.cs
using Lingrow.BusinessLogicLayer.DTOs.Schedule;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.BusinessLogicLayer.Mapping;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Services.Schedules;

public class StudentCardService : IStudentCardService
{
    private readonly IStudentCardRepo _cardRepo;

    public StudentCardService(IStudentCardRepo cardRepo)
    {
        _cardRepo = cardRepo;
    }

    public async Task<List<StudentCardResponse>> GetForTutorAsync(Guid tutorId)
    {
        LoggerHelper.Info($"GetForTutorAsync | tutorId={tutorId}");

        var cards = await _cardRepo.GetByTutorAsync(tutorId);
        return cards.Select(c => c.ToResponse()).ToList();
    }

    public async Task<StudentCardResponse> CreateAsync(
        Guid tutorId,
        CreateStudentCardRequest request
    )
    {
        LoggerHelper.Info($"CreateStudentCard | tutorId={tutorId}, name={request.DisplayName}");

        if (string.IsNullOrWhiteSpace(request.DisplayName))
            throw new ArgumentException("Display name is required.");

        // chuẩn hoá color
        var color = string.IsNullOrWhiteSpace(request.Color)
            ? "blue"
            : request.Color.Trim().ToLowerInvariant();

        var entity = new StudentCard
        {
            TutorId = tutorId,
            // KHÔNG dùng StudentId nữa
            DisplayName = request.DisplayName.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            Tags = request.Tags,
            Color = color,
        };

        var saved = await _cardRepo.CreateAsync(entity);

        LoggerHelper.Info($"CreateStudentCard success | cardId={saved.Id}");

        return saved.ToResponse();
    }
}
