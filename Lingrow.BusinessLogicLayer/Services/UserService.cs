using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.BusinessLogicLayer.Options;
using Lingrow.DataAccessLayer.Interface;
using Microsoft.Extensions.Options;

namespace Lingrow.BusinessLogicLayer.Auth;

public partial class UserService : IUserService
{
    private readonly IUserRepo _userRepo;
    private readonly AuthOptions _options;

    public UserService(IUserRepo userRepo, IOptions<AuthOptions> options)
    {
        _userRepo = userRepo;
        _options = options.Value;
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

        // 1. Kiểm tra tồn tại user + navigation
        var presenceError = ValidateUserPresence(ld, usernameOrEmail);
        if (presenceError is not null)
            return presenceError;

        var loginData = ld!;

        LoggerHelper.Info(
            $"Found user: {loginData.Username}, email: {loginData.Email}, "
                + $"role: {loginData.Role}, userId: {loginData.UserId}"
        );

        // 2. Kiểm tra trạng thái user
        var statusError = ValidateUserStatus(loginData);
        if (statusError is not null)
            return statusError;

        // 3. Kiểm tra lockout
        var lockoutError = ValidateLockout(loginData);
        if (lockoutError is not null)
            return lockoutError;

        // 4. Verify password
        var valid = PasswordHelper.VerifyPassword(
            password,
            loginData.PasswordHash,
            loginData.PasswordSalt
        );

        if (!valid)
            return await HandleInvalidPasswordAsync(loginData);

        // 5. Thành công
        await OnPasswordVerifiedAsync(loginData);

        // 6. Build kết quả
        return BuildSuccessResult(loginData);
    }
}
