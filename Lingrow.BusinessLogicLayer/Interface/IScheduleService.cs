using Lingrow.BusinessLogicLayer.DTOs;
using Lingrow.Models;

namespace Lingrow.BusinessLogicLayer.Interface;

public interface IScheduleService
{
    Task<Schedule> CreateScheduleAsync(
        Guid tutorId,
        CreateScheduleRequest request,
        CancellationToken cancellationToken = default
    );
}
