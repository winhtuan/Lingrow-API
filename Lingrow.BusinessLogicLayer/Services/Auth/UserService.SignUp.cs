using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.Enum;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Service.Auth;

public partial class UserService
{
    public async Task<UserAccount> SignUpAsync(
        string email,
        string fullName,
        DateOnly dateOfBirth,
        string password
    )
    {
        LoggerHelper.Info($"Signup request: email={email}, fullName={fullName}");

        try
        {
            // 1. Validate input cơ bản
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.", nameof(fullName));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.", nameof(password));

            // 2. Chuẩn hóa email
            email = email.Trim().ToLowerInvariant();

            // 3. Kiểm tra email đã tồn tại chưa
            if (await _userRepo.EmailExistsAsync(email))
            {
                LoggerHelper.Warn($"Signup failed: email already exists ({email})");
                throw new InvalidOperationException("Email already registered");
            }

            // 4. Tạo username từ email
            var username = email.Split('@')[0];

            // 5. Hash password (byte[])
            var (hash, salt) = PasswordHelper.GeneratePasswordHash(password);

            // 6. Tạo entity
            var account = BuildNewUserAccount(fullName, dateOfBirth);
            var loginData = BuildNewUserLoginData(account.UserId, username, email, hash, salt);

            // 7. Lưu DB qua UserRepo
            await _userRepo.AddUserAsync(account, loginData);
            await _userRepo.SaveChangesAsync();

            LoggerHelper.Info(
                $"Signup success: email={email}, username={username}, userId={account.UserId}"
            );

            return account;
        }
        catch (Exception ex)
        {
            LoggerHelper.Error(ex); // log full stacktrace vào file
            throw; // ném lại để API layer xử lý (return 400/500 tuỳ bạn)
        }
    }

    private static UserAccount BuildNewUserAccount(string fullName, DateOnly dob) =>
        new()
        {
            UserId = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Gender = 'U',
            DateOfBirth = dob,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
        };

    private static UserLoginData BuildNewUserLoginData(
        Guid userId,
        string username,
        string email,
        byte[] passwordHash,
        byte[] passwordSalt
    ) =>
        new()
        {
            UserId = userId,
            Username = username,
            Email = email,
            Role = Role.user,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = false,
        };
}
