using Bogus;
using Jedlix.Core;
using Jedlix.Core.Models;
using System;
using System.Collections.Generic;

namespace Jedlix.Tests.Extensions
{
    public static class Schedule_Extensions
    {
        public static Schedule Build(this Schedule instance)
        {
            instance = new Faker<Schedule>();
            return instance;
        }

        public static Schedule WithStartingTime(this Schedule instance, string startingTime)
        {
            instance.StartingTime = startingTime;
            return instance;
        }

        public static Schedule WithUserSettings(this Schedule instance)
        {
            instance.UserSettings = new UserSettings
            {
                DesiredStateOfCharge = 90,
                LeavingTime = new Randomizer().ReplaceNumbers("0#:0#"),
                DirectChargingPercentage = 30,
                Tariffs = new Faker<Tariffs>()
                    .RuleFor(t => t.StartTime, new Randomizer().ReplaceNumbers("0#:0#"))
                    .RuleFor(t => t.EndTime, new Randomizer().ReplaceNumbers("0#:0#"))
                    .RuleFor(t => t.EnergyPrice, decimal.Round(
                        new Randomizer().Decimal(min:0.01M, max:0.30M), 2, MidpointRounding.AwayFromZero)).Generate(5)
            };

            return instance;
        }

        public static Schedule WithCarData(this Schedule instance)
        {
            instance.CarData = new CarData
            {
                ChargePower = decimal.Round(new Randomizer().Decimal(min: 0.0M, max: 9.9M), 2, MidpointRounding.AwayFromZero),
                BatteryCapacity = decimal.Round(new Randomizer().Decimal(min: 30M, max: 70M), 2, MidpointRounding.AwayFromZero),
                CurrentBatteryLevel = decimal.Round(new Randomizer().Decimal(min: 10M, max: 30M), 2, MidpointRounding.AwayFromZero)
            };

            return instance;
        }

        public static Schedule WithInvalidStartingTime(this Schedule instance)
        {
            instance.StartingTime = string.Empty;
            return instance;
        }

        public static Schedule WithInvalidLeavingTime(this Schedule instance)
        {
            instance.UserSettings.LeavingTime = string.Empty;
            return instance;
        }

        public static Schedule WithoutTariffs(this Schedule instance)
        {
            instance.UserSettings.Tariffs = new List<Tariffs>();
            return instance;
        }

        public static Schedule WithInvalidTariff(this Schedule instance, Tariffs tariff)
        {
            instance.UserSettings.Tariffs = new List<Tariffs>() { tariff };
            return instance;
        }

        public static Schedule WithInvalidCarData(this Schedule instance, CarData carData)
        {
            instance.CarData = carData;
            return instance;
        }
    }
}