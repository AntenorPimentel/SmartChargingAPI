namespace Jedlix.Core.DTOs
{
    public class OptimalScheduleDto
    {
        public IEnumerable<ScheduleDto> ScheduleDtos { get; set; }
    }

    public class ScheduleDto
    {
        public string StartingTime { get; set; }
        public string EndingTime { get; set; }
        public bool IsCharging { get; set; }

        public ScheduleDto() { }

        public ScheduleDto(DateTime startingTime, DateTime endingTime, bool isCharging)
        {
            StartingTime = startingTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            EndingTime = endingTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            IsCharging = isCharging;
        }

        public ScheduleDto(string startingTime, DateTime endingTime, bool isCharging)
        {
            StartingTime = startingTime;
            EndingTime = endingTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
            IsCharging = isCharging;
        }
    }
}