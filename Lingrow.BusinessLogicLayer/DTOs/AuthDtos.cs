using System.ComponentModel.DataAnnotations;

namespace Lingrow.BusinessLogicLayer.DTOs;

public class AuthDtos
{
    public sealed record LoginRequest(string UsernameOrEmail, string Password);

    public sealed record AuthUserDto(Guid UserId, string Username, string Email, string Role);

    // thêm AccessToken để sau này gắn JWT
    public sealed record LoginResponse(AuthUserDto User, string? AccessToken = null);

    public sealed record SignUpRequest(
        [Required, EmailAddress] string Email,
        [Required, MaxLength(255)] string FullName,
        DateOnly DateOfBirth,
        [Required, MinLength(6)] string Password
    );

    public sealed record SignUpResponse(AuthUserDto User);
}
