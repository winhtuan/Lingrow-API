using System.Security.Claims;
using Lingrow.BusinessLogicLayer.DTOs;
using Lingrow.BusinessLogicLayer.Helper;
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

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var sub =
            User.FindFirst("sub")?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("cognito:username")?.Value;
        var email = User.FindFirst("email")?.Value;
        var username =
            User.FindFirst("cognito:username")?.Value ?? User.FindFirst("username")?.Value;
        var fullName = User.FindFirst("name")?.Value;
        var birthdate = ValueParser.ParseDateOnly(User.FindFirst("birthdate")?.Value);

        if (sub is null || email is null)
        {
            return Unauthorized(new { error = "Invalid Cognito token." });
        }

        var user = await _userService.SyncCognitoUserAsync(
            sub,
            email,
            username,
            fullName,
            birthdate
        );

        var dto = new AuthDtos.AuthUserDto(
            user.UserId,
            user.Username,
            user.Email,
            user.Role.ToString()
        );

        return Ok(dto);
    }

    [Authorize]
    [HttpGet("check")]
    public IActionResult CheckAuth()
    {
        var email = User.FindFirst("email")?.Value ?? "Unknown";
        return Ok(new { message = "Token valid", email });
    }
}
