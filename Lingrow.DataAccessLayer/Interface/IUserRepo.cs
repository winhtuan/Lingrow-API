using Lingrow.Models;

namespace Lingrow.DataAccessLayer.Interface;

public interface IUserRepo
{
    Task<UserAccount?> GetUserAsync(string input);

    Task<UserAccount?> GetByCognitoSubAsync(string cognitoSub);

    Task<bool> EmailExistsAsync(string email);

    Task AddUserAsync(UserAccount user);

    Task<int> SaveChangesAsync();
}
