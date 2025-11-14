using Lingrow.BusinessLogicLayer.Auth;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.Enum;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Service.Auth;

public partial class UserService
{
    private async Task<LoginResult> HandleInvalidPasswordAsync(UserLoginData ld)
    {
        ld.AccessFailedCount++;
        LoggerHelper.Warn(
            $"Invalid password for '{ld.Username}'. Failed attempts: {ld.AccessFailedCount}"
        );

        if (ld.AccessFailedCount >= _options.MaxFailedAccess)
        {
            ld.LockoutEnd = DateTime.UtcNow.AddMinutes(_options.LockoutMinutes);
            ld.AccessFailedCount = 0;
        }

        await _userRepo.SaveChangesAsync();

        return new LoginResult(
            Status: LoginStatus.InvalidPassword,
            ErrorMessage: "Invalid credentials"
        );
    }

    private async Task OnPasswordVerifiedAsync(UserLoginData ld)
    {
        ld.AccessFailedCount = 0;
        ld.LockoutEnd = null;
        ld.LastLoginAt = DateTime.UtcNow;

        await _userRepo.SaveChangesAsync();

        LoggerHelper.Info($"Login successful for '{ld.Username}' at {ld.LastLoginAt:u}");
    }

    private static LoginResult BuildSuccessResult(UserLoginData ld)
    {
        var authUser = new AuthUser(
            UserId: ld.UserId,
            Username: ld.Username,
            Email: ld.Email,
            Role: ld.Role.ToString()
        );

        // TODO: log UserActivity tại đây nếu cần (ip, userAgent, correlationId...)

        return new LoginResult(Status: LoginStatus.Success, User: authUser);
    }
}
