using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lingrow.Api.Utils;

public static class ExceptionUtil
{
    public static IActionResult ToResult(ControllerBase controller, Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => controller.NotFound(new { message = ex.Message }),

            InvalidOperationException => controller.BadRequest(new { message = ex.Message }),

            ArgumentException => controller.BadRequest(new { message = ex.Message }),

            UnauthorizedAccessException => controller.StatusCode(
                StatusCodes.Status401Unauthorized,
                new { message = ex.Message }
            ),

            _ => controller.StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "An unexpected error occurred." }
            ),
        };
    }
}
