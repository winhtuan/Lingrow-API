using Lingrow.Models;

namespace Lingrow.DataAccessLayer.Interface;

public interface IUserRepo
{
    Task<UserLoginData?> GetUserLoginDataAsync(string input);
    Task<bool> EmailExistsAsync(string email);
    Task AddUserAsync(UserAccount account, UserLoginData loginData);
    Task<int> SaveChangesAsync();
}
