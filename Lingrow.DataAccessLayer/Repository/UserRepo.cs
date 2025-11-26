using Lingrow.DataAccessLayer.Data;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Models;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.DataAccessLayer.Repository;

public class UserRepo : IUserRepo
{
    private readonly AppDbContext _context;

    public UserRepo(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lấy user theo ID
    /// </summary>
    public Task<UserAccount?> GetByIdAsync(Guid id)
    {
        return _context.UserAccounts.FirstOrDefaultAsync(u => u.UserId == id);
    }

    /// <summary>
    /// Lấy user theo CognitoSub (sub trong JWT token)
    /// </summary>
    public Task<UserAccount?> GetByCognitoSubAsync(string cognitoSub)
    {
        return _context
            .UserAccounts.AsTracking()
            .FirstOrDefaultAsync(u => u.CognitoSub == cognitoSub);
    }

    /// <summary>
    /// Kiểm tra email đã tồn tại hay chưa
    /// </summary>
    public async Task<UserAccount?> GetByEmailAsync(string email)
    {
        return await _context.UserAccounts.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Thêm user mới (khi user lần đầu đăng nhập qua Cognito)
    /// </summary>
    public async Task AddUserAsync(UserAccount user)
    {
        await _context.UserAccounts.AddAsync(user);
    }

    /// <summary>
    /// Lưu thay đổi database
    /// </summary>
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
