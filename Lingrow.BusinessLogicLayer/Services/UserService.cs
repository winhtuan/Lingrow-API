using Plantpedia.BusinessLogicLayer.Helper;
using Plantpedia.BusinessLogicLayer.Interface;
using Plantpedia.DataAccessLayer.Interface;
using Plantpedia.Models;

public class UserService : IUserService
{
    private readonly IUserRepo _userRepo;

    private const int MaxFailedAccess = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    public UserService(IUserRepo userRepo) => _userRepo = userRepo;

    public async Task<UserAccount> LoginAsync(string usernameOrEmail, string password)
    {
        LoggerHelper.Info($"Login request from '{usernameOrEmail}'");

        var ld = await _userRepo.GetUserLoginDataAsync(usernameOrEmail);
        if (ld == null)
        {
            LoggerHelper.Warn($"User '{usernameOrEmail}' not found");
            throw new Exception("User not found");
        }

        LoggerHelper.Info($"Found user: {ld.Username}, email: {ld.Email}, role: {ld.Role}");

        // kiểm tra lockout
        if (ld.LockoutEnd.HasValue && ld.LockoutEnd.Value > DateTime.UtcNow)
        {
            LoggerHelper.Warn($"Account '{ld.Username}' is locked until {ld.LockoutEnd.Value:u}");
            throw new Exception("Account locked. Please try again later.");
        }

        var valid = PasswordHelper.VerifyPassword(password, ld.PasswordHash, ld.PasswordSalt);
        if (!valid)
        {
            ld.AccessFailedCount++;
            LoggerHelper.Warn(
                $"Invalid password for '{ld.Username}'. Failed attempts: {ld.AccessFailedCount}"
            );

            if (ld.AccessFailedCount >= MaxFailedAccess)
            {
                ld.LockoutEnd = DateTime.UtcNow + LockoutDuration;
                ld.AccessFailedCount = 0;
                LoggerHelper.Warn($"Account '{ld.Username}' locked until {ld.LockoutEnd.Value:u}");
            }

            await _userRepo.SaveChangesAsync();
            throw new Exception("Invalid password");
        }

        // Đăng nhập thành công
        ld.AccessFailedCount = 0;
        ld.LockoutEnd = null;
        ld.LastLoginAt = DateTime.UtcNow;
        await _userRepo.SaveChangesAsync();

        LoggerHelper.Info($"Login successful for '{ld.Username}' at {ld.LastLoginAt:u}");

        if (ld.User is null)
        {
            LoggerHelper.Warn($"User navigation not loaded for '{ld.Username}'");
            throw new Exception("User navigation not loaded");
        }

        return ld.User;
    }
}
