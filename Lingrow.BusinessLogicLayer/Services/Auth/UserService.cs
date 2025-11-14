using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.BusinessLogicLayer.Options;
using Lingrow.DataAccessLayer.Interface;
using Microsoft.Extensions.Options;

namespace Lingrow.BusinessLogicLayer.Service.Auth;

public partial class UserService : IUserService
{
    private readonly IUserRepo _userRepo;
    private readonly AuthOptions _options;

    public UserService(IUserRepo userRepo, IOptions<AuthOptions> options)
    {
        _userRepo = userRepo;
        _options = options.Value;
    }
}
