using Lingrow.Models;

namespace Lingrow.DataAccessLayer.Interface;

public interface IUserRepo
{
    Task<UserLoginData?> GetUserLoginDataAsync(string input);
    Task<int> SaveChangesAsync();
}
