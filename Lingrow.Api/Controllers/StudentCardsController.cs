// Lingrow.Api/Controllers/StudentCardsController.cs
using Lingrow.Api.Extensions;
using Lingrow.BusinessLogicLayer.DTOs.Schedule;
using Lingrow.BusinessLogicLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lingrow.Api.Controllers;

[ApiController]
[Route("api/student-cards")]
[Authorize]
public class StudentCardsController : ControllerBase
{
    private readonly IStudentCardService _service;

    public StudentCardsController(IStudentCardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMine()
    {
        try
        {
            var tutorId = User.GetUserId();
            var result = await _service.GetForTutorAsync(tutorId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentCardRequest request)
    {
        try
        {
            var tutorId = User.GetUserId();
            var result = await _service.CreateAsync(tutorId, request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    private IActionResult HandleException(Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => NotFound(new { message = ex.Message }),
            InvalidOperationException => BadRequest(new { message = ex.Message }),
            ArgumentException => BadRequest(new { message = ex.Message }),
            UnauthorizedAccessException => StatusCode(
                StatusCodes.Status401Unauthorized,
                new { message = ex.Message }
            ),
            _ => StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            ),
        };
    }
}
