using Lingrow.BusinessLogicLayer.DTOs;
using Lingrow.BusinessLogicLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lingrow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Lấy thông tin user hiện tại từ Cognito JWT token.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var sub = User.FindFirst("sub")?.Value;
        var email = User.FindFirst("email")?.Value;
        var username = User.FindFirst("cognito:username")?.Value;
        var fullName = User.FindFirst("name")?.Value;

        if (sub is null || email is null)
        {
            return Unauthorized(new { error = "Invalid Cognito token." });
        }

        // Đồng bộ user vào DB nếu chưa tồn tại
        var user = await _userService.SyncCognitoUserAsync(sub, email, username, fullName);

        var dto = new AuthDtos.AuthUserDto(
            user.UserId,
            user.Username,
            user.Email,
            user.Role.ToString()
        );

        return Ok(dto);
    }

    /// <summary>
    /// Endpoint test token validity (cho FE ping thử)
    /// </summary>
    [Authorize]
    [HttpGet("check")]
    public IActionResult CheckAuth()
    {
        var email = User.FindFirst("email")?.Value ?? "Unknown";
        return Ok(new { message = "Token valid", email });
    }
}
