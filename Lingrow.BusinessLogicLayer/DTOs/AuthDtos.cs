namespace Lingrow.BusinessLogicLayer.DTOs;

public class AuthDtos
{
    public sealed record LoginRequest(string UsernameOrEmail, string Password);

    public sealed record AuthUserDto(long UserId, string Username, string Email, string Role);

    // thêm AccessToken để sau này gắn JWT
    public sealed record LoginResponse(AuthUserDto User, string? AccessToken = null);
}
