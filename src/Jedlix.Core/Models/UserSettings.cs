namespace Jedlix.Core.Models
{
    public class UserSettings
    {
        public int DesiredStateOfCharge { get; set; }
        public string LeavingTime { get; set; }
        public int DirectChargingPercentage { get; set; }
        public IEnumerable<Tariffs> Tariffs { get; set; }
    }
}