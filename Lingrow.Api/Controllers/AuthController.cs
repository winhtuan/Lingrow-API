using Lingrow.BusinessLogicLayer.DTOs;
using Lingrow.BusinessLogicLayer.Interface;
using Lingrow.Enum;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDtos.LoginRequest req)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // lấy metadata để sau này log activity
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var ua = Request.Headers.UserAgent.ToString();
        var correlationId = HttpContext.TraceIdentifier;

        var result = await _userService.LoginAsync(
            req.UsernameOrEmail,
            req.Password,
            ip,
            ua,
            correlationId
        );

        if (!result.Succeeded)
        {
            return result.Status switch
            {
                LoginStatus.UserNotFound => NotFound(
                    new { error = result.ErrorMessage ?? "User not found" }
                ),

                LoginStatus.InvalidPassword => Unauthorized(
                    new { error = result.ErrorMessage ?? "Invalid credentials" }
                ),

                LoginStatus.LockedOut => StatusCode(
                    StatusCodes.Status423Locked,
                    new { error = result.ErrorMessage ?? "Account locked" }
                ),

                LoginStatus.Suspended => StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { error = result.ErrorMessage ?? "Account suspended" }
                ),

                LoginStatus.Deleted => StatusCode(
                    StatusCodes.Status410Gone,
                    new { error = result.ErrorMessage ?? "Account deleted" }
                ),

                LoginStatus.EmailNotConfirmed => StatusCode(
                    StatusCodes.Status403Forbidden,
                    new { error = result.ErrorMessage ?? "Email not confirmed" }
                ),

                _ => BadRequest(new { error = result.ErrorMessage ?? "Login failed" }),
            };
        }

        // map AuthUser (domain) -> AuthUserDto (API)
        var u = result.User!;
        var userDto = new AuthDtos.AuthUserDto(
            UserId: u.UserId,
            Username: u.Username,
            Email: u.Email,
            Role: u.Role
        );

        // TODO: sau này generate JWT ở đây
        // var token = _jwtService.GenerateToken(u);

        var response = new AuthDtos.LoginResponse(
            userDto /*, token */
        );

        return Ok(response);
    }
}
