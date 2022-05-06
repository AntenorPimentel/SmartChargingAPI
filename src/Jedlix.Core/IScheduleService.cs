using Jedlix.Core.DTOs;

namespace Jedlix.Core
{
    public interface IScheduleService
    {
        Task<OptimalScheduleDto> CalculateOptimalSchedule(Schedule scheduleDto);
    }
}