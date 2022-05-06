using Jedlix.Core;
using Microsoft.AspNetCore.Mvc;

namespace Jedlix.Api.Controllers
{
    [ApiController]
    public class ChargeProfileController : ControllerBase
    {
        private readonly ILogger<ChargeProfileController> _logger;
        private readonly IScheduleService _scheduleService;

        public ChargeProfileController(ILogger<ChargeProfileController> logger, IScheduleService scheduleService)
        {
            _logger = logger;
            _scheduleService = scheduleService;
        }

        [HttpPost]
        [Route("OptimizeSchedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CalculateOptimalSchedule([FromBody] Schedule schedule)
        {
            try
            {
                return Ok(await _scheduleService.CalculateOptimalSchedule(schedule));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error Generating Charge Profile Schedule", schedule);
                return NotFound(ex.Message);
            }
        }
    }
}