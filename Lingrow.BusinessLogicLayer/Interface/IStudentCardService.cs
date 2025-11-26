using Lingrow.BusinessLogicLayer.DTOs.Schedule;

namespace Lingrow.BusinessLogicLayer.Interface;

public interface IStudentCardService
{
    Task<StudentCardResponse> CreateAsync(Guid tutorId, CreateStudentCardRequest request);
    Task<List<StudentCardResponse>> GetForTutorAsync(Guid tutorId);
}
