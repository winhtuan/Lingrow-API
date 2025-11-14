using Lingrow.BusinessLogicLayer.Auth;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.Enum;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Service.Auth;

public partial class UserService
{
    public async Task<LoginResult> LoginAsync(
        string usernameOrEmail,
        string password,
        string? ip = null,
        string? userAgent = null,
        string? correlationId = null
    )
    {
        LoggerHelper.Info(
            $"Login request: userOrEmail={usernameOrEmail}, ip={ip}, ua={userAgent}, cid={correlationId}"
        );

        // Validate input cơ bản (tránh null/empty)
        if (string.IsNullOrWhiteSpace(usernameOrEmail))
            throw new ArgumentException("Username or email is required.", nameof(usernameOrEmail));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        try
        {
            // Lấy login data từ repo (có Include User + tracking)
            var ld = await _userRepo.GetUserLoginDataAsync(usernameOrEmail);

            // Chạy toàn bộ guard (user có tồn tại, status hợp lệ, không bị lockout)
            var guardError = RunLoginGuards(ld, usernameOrEmail);
            if (guardError is not null)
                return guardError;

            var loginData = ld!;

            LoggerHelper.Info(
                $"Login found user: {loginData.Username}, email={loginData.Email}, role={loginData.Role}, userId={loginData.UserId}"
            );

            // Verify password
            var valid = PasswordHelper.VerifyPassword(
                password,
                loginData.PasswordHash,
                loginData.PasswordSalt
            );

            if (!valid)
                return await HandleInvalidPasswordAsync(loginData);

            await OnPasswordVerifiedAsync(loginData);

            return BuildSuccessResult(loginData);
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex);
            throw; // để controller layer xử lý trả 500 hoặc message chung
        }
    }

    // ====== GUARD DÙNG CHUNG CHO LOGIN ======

    private static LoginResult? RunLoginGuards(UserLoginData? ld, string usernameOrEmail)
    {
        // 1. Tồn tại user + navigation
        var presenceError = ValidateUserPresence(ld, usernameOrEmail);
        if (presenceError is not null)
            return presenceError;

        var loginData = ld!;

        // 2. Trạng thái user (Active / Suspended / Deleted)
        var statusError = ValidateUserStatus(loginData);
        if (statusError is not null)
            return statusError;

        // 3. Lockout
        var lockoutError = ValidateLockout(loginData);
        if (lockoutError is not null)
            return lockoutError;

        return null;
    }

    private static LoginResult? ValidateUserPresence(UserLoginData? ld, string usernameOrEmail)
    {
        if (ld is null)
        {
            LoggerHelper.Warn($"User '{usernameOrEmail}' not found");
            return new LoginResult(
                Status: LoginStatus.UserNotFound,
                ErrorMessage: "User not found"
            );
        }

        if (ld.User is null)
        {
            LoggerHelper.Warn($"User navigation not loaded for '{ld.Username}'");
            return new LoginResult(
                Status: LoginStatus.Deleted,
                ErrorMessage: "User is no longer available"
            );
        }

        return null;
    }

    private static LoginResult? ValidateUserStatus(UserLoginData ld)
    {
        if (ld.User.Status == UserStatus.Suspended)
        {
            LoggerHelper.Warn($"User '{ld.Username}' is suspended");
            return new LoginResult(
                Status: LoginStatus.Suspended,
                ErrorMessage: "Account suspended"
            );
        }

        if (ld.User.Status == UserStatus.Deleted)
        {
            LoggerHelper.Warn($"User '{ld.Username}' is deleted");
            return new LoginResult(Status: LoginStatus.Deleted, ErrorMessage: "Account deleted");
        }

        return null;
    }

    private static LoginResult? ValidateLockout(UserLoginData ld)
    {
        if (ld.LockoutEnd.HasValue && ld.LockoutEnd.Value > DateTime.UtcNow)
        {
            LoggerHelper.Warn($"Account '{ld.Username}' is locked until {ld.LockoutEnd.Value:u}");

            return new LoginResult(
                Status: LoginStatus.LockedOut,
                ErrorMessage: "Account locked. Please try again later."
            );
        }

        return null;
    }
}
