using Lingrow.Models;

namespace Lingrow.DataAccessLayer.Interface;

public interface IUserRepo
{
    Task<UserAccount?> GetByCognitoSubAsync(string cognitoSub);
    Task<UserAccount?> GetByEmailAsync(string email);
    Task AddUserAsync(UserAccount user);
    Task SaveChangesAsync();
}
