namespace Plantpedia.Object.DTOs;

public class AuthDtos
{
    public sealed record LoginRequest(string UsernameOrEmail, string Password);

    public sealed record AuthUserDto(long UserId, string Username, string Email, string Role);

    public sealed record LoginResponse(AuthUserDto User);
}
