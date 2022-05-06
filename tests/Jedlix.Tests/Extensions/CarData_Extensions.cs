using Bogus;
using Jedlix.Core.Models;
using System;

namespace Jedlix.Tests.Extensions
{
    public static class CarData_Extensions
    {
        public static CarData Build(this CarData instance)
        {
            instance = new Faker<CarData>()
                .RuleFor(t => t.ChargePower, decimal.Round(
                        new Randomizer().Decimal(min: 0.00M, max: 100.00M), 2, MidpointRounding.AwayFromZero))
                .RuleFor(t => t.BatteryCapacity, decimal.Round(
                        new Randomizer().Decimal(min: 0.00M, max: 100.00M), 2, MidpointRounding.AwayFromZero))
                .RuleFor(t => t.CurrentBatteryLevel, decimal.Round(
                        new Randomizer().Decimal(min: 0.00M, max: 100.00M), 2, MidpointRounding.AwayFromZero));

            return instance;
        }

        public static CarData WithInvalidChargePower(this CarData instance)
        {
            instance.ChargePower = -9.6M;

            return instance;
        }

        public static CarData WithInvalidCurrentBatteryLevel(this CarData instance)
        {
            instance.CurrentBatteryLevel = decimal.Zero;

            return instance;
        }

        public static CarData WithInvalidBatteryCapacity(this CarData instance)
        {
            instance.BatteryCapacity = decimal.Zero;

            return instance;
        }
    }
}