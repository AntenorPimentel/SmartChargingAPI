using Jedlix.Core.Models;

namespace Jedlix.Core
{
    public class Schedule
    {
        public string StartingTime { get; set; }
        public UserSettings UserSettings { get; set; }
        public CarData CarData { get; set; }

    }
}