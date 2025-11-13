namespace Lingrow.BusinessLogicLayer.Auth;

public sealed record AuthUser(long UserId, string Username, string Email, string Role);
