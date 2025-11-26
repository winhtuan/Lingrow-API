using Lingrow.Models;

namespace Lingrow.DataAccessLayer.Interface;

public interface IStudentCardRepo
{
    Task<StudentCard?> GetByIdAsync(Guid id);
}
