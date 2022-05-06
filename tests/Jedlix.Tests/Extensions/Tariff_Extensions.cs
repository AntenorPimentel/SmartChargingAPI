using Bogus;
using Jedlix.Core.Models;

namespace Jedlix.Tests.Extensions
{
    public static class Tariff_Extensions
    {
        public static Tariffs Build(this Tariffs instance)
        {
            instance = new Faker<Tariffs>()
                .RuleFor(t => t.StartTime, "00:00")
                .RuleFor(t => t.EndTime, "07:15")
                .RuleFor(t => t.EnergyPrice, 0.22M);

            return instance;
        }

        public static Tariffs WithInvalidStartTime(this Tariffs instance)
        {
            instance.StartTime = string.Empty;

            return instance;
        }

        public static Tariffs WithInvalidEndTime(this Tariffs instance)
        {
            instance.EndTime = string.Empty;

            return instance;
        }
    }
}