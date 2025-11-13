using Lingrow.DataAccessLayer.Data;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepo : IUserRepo
{
    private readonly AppDbContext _context;

    public UserRepo(AppDbContext context) => _context = context;

    public Task<UserLoginData?> GetUserLoginDataAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));

        // cần tracking để set LastLoginAt, AccessFailedCount
        var query = _context.UserLoginDatas.Include(u => u.User).AsTracking();

        return input.Contains('@')
            ? query.FirstOrDefaultAsync(u => u.Email == input)
            : query.FirstOrDefaultAsync(u => u.Username == input);
    }

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
}
