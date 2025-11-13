using Lingrow.BusinessLogicLayer.Auth;
using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.DataAccessLayer.Interface;
using Lingrow.Enum;

public class UserService : IUserService
{
    private readonly IUserRepo _userRepo;

    // config lockout – sau này có thể đưa vào appsettings + options pattern
    private const int MaxFailedAccess = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    public UserService(IUserRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<LoginResult> LoginAsync(
        string usernameOrEmail,
        string password,
        string? ip = null,
        string? userAgent = null,
        string? correlationId = null
    )
    {
        LoggerHelper.Info($"Login request from '{usernameOrEmail}'");

        var ld = await _userRepo.GetUserLoginDataAsync(usernameOrEmail);

        if (ld is null)
        {
            LoggerHelper.Warn($"User '{usernameOrEmail}' not found");
            return new LoginResult(
                Status: LoginStatus.UserNotFound,
                ErrorMessage: "User not found"
            );
        }

        LoggerHelper.Info(
            $"Found user: {ld.Username}, email: {ld.Email}, role: {ld.Role}, userId: {ld.UserId}"
        );

        // check user entity (status, soft-delete)
        if (ld.User is null)
        {
            LoggerHelper.Warn($"User navigation not loaded for '{ld.Username}'");
            return new LoginResult(
                Status: LoginStatus.Deleted,
                ErrorMessage: "User is no longer available"
            );
        }

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

        // lockout
        if (ld.LockoutEnd.HasValue && ld.LockoutEnd.Value > DateTime.UtcNow)
        {
            LoggerHelper.Warn($"Account '{ld.Username}' is locked until {ld.LockoutEnd.Value:u}");
            return new LoginResult(
                Status: LoginStatus.LockedOut,
                ErrorMessage: "Account locked. Please try again later."
            );
        }

        // verify password
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

            return new LoginResult(
                Status: LoginStatus.InvalidPassword,
                ErrorMessage: "Invalid credentials"
            );
        }

        // check email confirmed (nếu muốn bắt buộc)
        // if (!ld.EmailConfirmed)
        // {
        //     LoggerHelper.Warn($"Email not confirmed for '{ld.Username}'");
        //     return new LoginResult(
        //         Status: LoginStatus.EmailNotConfirmed,
        //         ErrorMessage: "Email not confirmed"
        //     );
        // }

        // success: reset lockout + update last login
        ld.AccessFailedCount = 0;
        ld.LockoutEnd = null;
        ld.LastLoginAt = DateTime.UtcNow;

        await _userRepo.SaveChangesAsync();

        LoggerHelper.Info($"Login successful for '{ld.Username}' at {ld.LastLoginAt:u}");

        var authUser = new AuthUser(
            UserId: ld.UserId,
            Username: ld.Username,
            Email: ld.Email,
            Role: ld.Role.ToString()
        );

        // TODO: ở đây có thể log UserActivity nếu muốn, dùng ip/userAgent/correlationId

        return new LoginResult(Status: LoginStatus.Success, User: authUser);
    }
}
