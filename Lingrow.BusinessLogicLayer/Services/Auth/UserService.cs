using Lingrow.BusinessLogicLayer.Helper;
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
        string? fullName = null,
        DateOnly? birthdate = null
    )
    {
        try
        {
            LoggerHelper.Info(
                $"Sync request: sub={cognitoSub}, email={email}, username={username}, fullName={fullName}, birthdate={birthdate}"
            );

            if (string.IsNullOrWhiteSpace(cognitoSub))
                throw new ArgumentException("CognitoSub is required", nameof(cognitoSub));
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required", nameof(email));

            email = email.ToLowerInvariant();

            // 1. Check by CognitoSub
            var user = await _userRepo.GetByCognitoSubAsync(cognitoSub);
            if (user != null)
            {
                LoggerHelper.Info($"User found by CognitoSub: {user.UserId}");
                return user;
            }

            // 2. If not exist → check by Email
            user = await _userRepo.GetByEmailAsync(email);
            if (user != null)
            {
                LoggerHelper.Info($"User found by Email {email}: {user.UserId}. Updating sub...");

                user.CognitoSub = cognitoSub;

                if (!string.IsNullOrEmpty(fullName) && string.IsNullOrEmpty(user.FullName))
                {
                    user.FullName = fullName;
                    LoggerHelper.Info($"Updated FullName: {fullName}");
                }

                if (string.IsNullOrEmpty(user.Username))
                {
                    user.Username = username ?? email.Split('@')[0];
                    LoggerHelper.Info($"Updated Username: {user.Username}");
                }

                if (birthdate.HasValue && user.DateOfBirth is null)
                {
                    user.DateOfBirth = birthdate;
                    LoggerHelper.Info($"Updated DOB: {birthdate}");
                }

                await _userRepo.SaveChangesAsync();
                return user;
            }

            // 3. Create new user
            LoggerHelper.Info("No existing user found. Creating new user...");

            user = new UserAccount
            {
                CognitoSub = cognitoSub,
                Email = email,
                Username = username ?? email.Split('@')[0],
                FullName = fullName,
                Role = Role.user,
                Status = UserStatus.Active,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                DateOfBirth = birthdate,
                Gender = 'M',
            };

            await _userRepo.AddUserAsync(user);
            await _userRepo.SaveChangesAsync();

            LoggerHelper.Info($"New user created: {user.UserId}");
            return user;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex);
            throw;
        }
    }

    public Task<UserAccount?> GetByCognitoSubAsync(string cognitoSub)
    {
        LoggerHelper.Info($"Fetching user by sub={cognitoSub}");
        return _userRepo.GetByCognitoSubAsync(cognitoSub);
    }
}
