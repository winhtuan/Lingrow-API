using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.Enum;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Auth;

public partial class UserService
{
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
