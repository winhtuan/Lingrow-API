using Lingrow.BusinessLogicLayer.Auth;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Interface;

public interface IUserService
{
    Task<LoginResult> LoginAsync(
        string usernameOrEmail,
        string password,
        string? ip = null,
        string? userAgent = null,
        string? correlationId = null
    );

    Task<UserAccount> SignUpAsync(
        string email,
        string fullName,
        DateOnly dateOfBirth,
        string password
    );
}
