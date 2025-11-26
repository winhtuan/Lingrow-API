using Lingrow.Models;

namespace Lingrow.DataAccessLayer.Interface;

public interface IStudentCardRepo
{
    Task<StudentCard?> GetByIdAsync(Guid id);
    Task<StudentCard> CreateAsync(StudentCard card);
    Task<List<StudentCard>> GetByTutorAsync(Guid tutorId);
}
