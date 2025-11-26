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

    public async Task<List<StudentCard>> GetByTutorAsync(Guid tutorId)
    {
        return await _context.StudentCards.Where(c => c.TutorId == tutorId).ToListAsync();
    }

    public async Task<StudentCard> CreateAsync(StudentCard card)
    {
        _context.StudentCards.Add(card);
        await _context.SaveChangesAsync();

        return await _context.StudentCards.FirstAsync(x => x.Id == card.Id);
    }

    public Task<StudentCard?> GetByIdAsync(Guid id) =>
        _context.StudentCards.FirstOrDefaultAsync(x => x.Id == id);
}
