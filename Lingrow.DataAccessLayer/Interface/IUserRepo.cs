using Plantpedia.Models;

namespace Plantpedia.DataAccessLayer.Interface;

public interface IUserRepo
{
    Task<UserLoginData?> GetUserLoginDataAsync(string input);
    Task<int> SaveChangesAsync();
}
