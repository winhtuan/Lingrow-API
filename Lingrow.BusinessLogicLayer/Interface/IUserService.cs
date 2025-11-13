using Plantpedia.Models;

namespace Plantpedia.BusinessLogicLayer.Interface;

public interface IUserService
{
    Task<UserAccount> LoginAsync(string username, string password);
}
