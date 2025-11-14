namespace Lingrow.BusinessLogicLayer.Auth;

public sealed record AuthUser(Guid UserId, string Username, string Email, string Role);
