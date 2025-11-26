using System.ComponentModel.DataAnnotations;

namespace Lingrow.BusinessLogicLayer.DTOs.Schedule;

public class CreateStudentCardRequest
{
    public string DisplayName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? Color { get; set; }
    public string? Tags { get; set; }
}

public class StudentCardResponse
{
    public Guid Id { get; set; }
    public Guid TutorId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? Tags { get; set; }
    public string Color { get; set; } = "blue";
}
