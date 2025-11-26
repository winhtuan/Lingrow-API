using System.Security.Claims;
using Lingrow.BusinessLogicLayer.DTOs.Schedule;
using Lingrow.BusinessLogicLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lingrow.Api.Controllers;

[ApiController]
[Route("api/schedules")]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _service;

    public ScheduleController(IScheduleService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
    {
        var tutorId = GetTutorId();
        var result = await _service.CreateScheduleAsync(tutorId, request);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateScheduleRequest request)
    {
        var tutorId = GetTutorId();
        var result = await _service.UpdateScheduleAsync(tutorId, id, request);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tutorId = GetTutorId();
        var success = await _service.DeleteScheduleAsync(tutorId, id);
        return Ok(new { deleted = success });
    }

    private Guid GetTutorId()
    {
        var id = User.FindFirst("user_id")?.Value ?? throw new Exception("Missing user_id claim.");
        return Guid.Parse(id);
    }
}
