using Lingrow.DataAccessLayer.Data;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.DataAccessLayer.Repository;

public class StudentCardRepo : IStudentCardRepo
{
    private readonly AppDbContext _context;

    public StudentCardRepo(AppDbContext db)
    {
        _context = db;
    }

    public Task<StudentCard?> GetByIdAsync(Guid id) =>
        _context.StudentCards.FirstOrDefaultAsync(x => x.Id == id);
}
