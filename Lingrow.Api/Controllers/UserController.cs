using Lingrow.BusinessLogicLayer.Helper;
using Lingrow.BusinessLogicLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lingrow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly S3Helper _s3;

    public UserController(IUserService userService, S3Helper s3)
    {
        _userService = userService;
        _s3 = s3;
    }

    [Authorize]
    [HttpGet("avatar")]
    public async Task<IActionResult> GetAvatar()
    {
        // Lấy CognitoSub từ JWT
        var sub = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(sub))
            return Unauthorized();

        // Lấy user DB
        var user = await _userService.GetByCognitoSubAsync(sub);
        if (user == null)
            return NotFound(new { error = "User not found" });

        // Nếu chưa có avatar, trả null hoặc default
        if (string.IsNullOrEmpty(user.AvatarUrl))
            return Ok(new { url = "/default-avatar.png" });

        // Sinh Presigned URL để FE dùng hiển thị
        var key = S3Helper.GetAvatarKey(user.UserId);
        var presignedUrl = _s3.GetPresignedGetUrl(key, expiresInMinutes: 60);

        return Ok(new { url = presignedUrl });
    }
}
