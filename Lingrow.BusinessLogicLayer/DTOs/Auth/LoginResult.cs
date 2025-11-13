using Lingrow.Enum;

namespace Lingrow.BusinessLogicLayer.Auth;

public sealed record LoginResult(
    LoginStatus Status,
    AuthUser? User = null,
    string? ErrorMessage = null
)
{
    public bool Succeeded => Status == LoginStatus.Success;
}
