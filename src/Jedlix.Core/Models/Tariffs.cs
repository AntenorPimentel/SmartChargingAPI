using System.Text.Json.Serialization;

namespace Jedlix.Core.Models
{
    public class Tariffs
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public decimal EnergyPrice { get; set; }
        [JsonIgnore]
        public TimeSpan TimeSlot { get; set; }

        public Tariffs() { }

        public Tariffs(string startTime, string endTime, decimal energyPrice)
        {
            StartTime = startTime;
            EndTime = endTime;
            EnergyPrice = energyPrice;
        }
    }
}