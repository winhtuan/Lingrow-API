using Microsoft.AspNetCore.Mvc;

namespace Lingrow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping() => Ok("OK");
}
