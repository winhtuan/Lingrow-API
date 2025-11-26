using Lingrow.BusinessLogicLayer.DTOs.Schedule;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Mapping;

public static class StudentCardMapping
{
    public static StudentCardResponse ToResponse(this StudentCard card)
    {
        return new StudentCardResponse
        {
            Id = card.Id,
            TutorId = card.TutorId,
            DisplayName = card.DisplayName,
            Notes = card.Notes,
            Tags = card.Tags,
            Color = card.Color,
        };
    }
}
