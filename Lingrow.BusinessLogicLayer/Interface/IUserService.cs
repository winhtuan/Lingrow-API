using Lingrow.BusinessLogicLayer.Auth;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Interface;

public interface IUserService
{
    /// <summary>
    /// Đồng bộ thông tin người dùng từ Cognito
    /// </summary>
    Task<UserAccount> SyncCognitoUserAsync(
        string cognitoSub,
        string email,
        string? username = null,
        string? fullName = null
    );

    /// <summary>
    /// Lấy thông tin user theo CognitoSub
    /// </summary>
    Task<UserAccount?> GetByCognitoSubAsync(string cognitoSub);
}
