using Microsoft.AspNetCore.Mvc;
using Plantpedia.BusinessLogicLayer.Interface;
using Plantpedia.Object.DTOs;

namespace Plantpedia.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService) => _userService = userService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDtos.LoginRequest req)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var user = await _userService.LoginAsync(req.UsernameOrEmail, req.Password);

            var dto = new AuthDtos.AuthUserDto(
                UserId: user.UserId,
                Username: user.LoginData.Username,
                Email: user.LoginData.Email,
                Role: user.LoginData.Role.ToString()
            );

            return Ok(new AuthDtos.LoginResponse(dto));
        }
        catch (Exception ex)
        {
            // Phân loại nhanh theo nội dung Exception
            if (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(new { error = "User not found" });

            if (ex.Message.Contains("Invalid password", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { error = "Invalid credentials" });

            if (ex.Message.Contains("locked", StringComparison.OrdinalIgnoreCase))
                return StatusCode(StatusCodes.Status423Locked, new { error = ex.Message });

            return Problem(title: "Login failed", detail: ex.Message, statusCode: 400);
        }
    }
}
