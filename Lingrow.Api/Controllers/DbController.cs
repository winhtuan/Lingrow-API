using Lingrow.DataAccessLayer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lingrow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbController : ControllerBase
{
    private readonly AppDbContext _db;

    public DbController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
    {
        try
        {
            // test kết nối DB
            var canConnect = await _db.Database.CanConnectAsync();
            if (!canConnect)
            {
                return StatusCode(503, "Database not ready");
            }

            return Ok("OK");
        }
        catch (Exception ex)
        {
            // log nếu cần
            return StatusCode(503, $"Database error: {ex.Message}");
        }
    }
}
