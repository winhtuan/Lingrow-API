using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Enum;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Service.Auth;

public class UserService : IUserService
{
    private readonly IUserRepo _userRepo;

    public UserService(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    /// <summary>
    /// Đồng bộ user từ Cognito vào DB khi user đăng nhập lần đầu.
    /// </summary>
    public async Task<UserAccount> SyncCognitoUserAsync(
        string cognitoSub,
        string email,
        string? username = null,
        string? fullName = null
    )
    {
        if (string.IsNullOrWhiteSpace(cognitoSub))
            throw new ArgumentException("CognitoSub is required", nameof(cognitoSub));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required", nameof(email));

        // Kiểm tra user đã tồn tại chưa
        var existingUser = await _userRepo.GetByCognitoSubAsync(cognitoSub);
        if (existingUser != null)
            return existingUser;

        // Nếu chưa tồn tại → tạo mới
        var user = new UserAccount
        {
            CognitoSub = cognitoSub,
            Email = email.ToLowerInvariant(),
            Username = username ?? email.Split('@')[0],
            FullName = fullName,
            Role = Role.user,
            Status = UserStatus.Active,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepo.AddUserAsync(user);
        await _userRepo.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Lấy user theo CognitoSub
    /// </summary>
    public Task<UserAccount?> GetByCognitoSubAsync(string cognitoSub)
    {
        return _userRepo.GetByCognitoSubAsync(cognitoSub);
    }
}
