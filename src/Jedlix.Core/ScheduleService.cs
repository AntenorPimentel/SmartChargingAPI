using Jedlix.Core.DTOs;
using Jedlix.Core.Models;
using Jedlix.Core.Validators;

namespace Jedlix.Core
{
    public class ScheduleService : IScheduleService
    {
        public ScheduleService() { }

        public async Task<OptimalScheduleDto> CalculateOptimalSchedule(Schedule schedule)
        {
            IsValidChargeProfileSchedule(schedule);

            var scheduleDtos = new List<ScheduleDto>();
            var currentBatteryLevel = schedule.CarData.CurrentBatteryLevel;
            var directChargingKwh = decimal.Multiply(schedule.CarData.BatteryCapacity / 100M,  schedule.UserSettings.DirectChargingPercentage);

            if (CanStartImmediatelyCharge(currentBatteryLevel, directChargingKwh))
                scheduleDtos.Add(StartsImmediateChargeProcess(schedule.CarData, schedule.StartingTime, directChargingKwh, out currentBatteryLevel));

            var totalChargeTimeHour = CalculateTotalChargeTimeHour(schedule.CarData, schedule.UserSettings, currentBatteryLevel);
            var startingTime = DateTime.Parse(schedule.StartingTime);
            var leavingTime = DateTime.Parse(schedule.UserSettings.LeavingTime);

            var scheduleDtoList = GenerateChargingSchedule(schedule.UserSettings.Tariffs, totalChargeTimeHour, startingTime, leavingTime);
            scheduleDtoList.ToList().ForEach(scheduleDto => scheduleDtos.Add(scheduleDto));

            return await Task.Run(() => new OptimalScheduleDto() { ScheduleDtos = SortByStartTime(scheduleDtos) });
        }

        private static bool CanStartImmediatelyCharge(decimal currentBatteryLevel, decimal directChargingKwh) =>
            currentBatteryLevel < directChargingKwh;

        private static TimeSpan CalculateTotalChargeTimeHour(CarData carData, UserSettings userSettings, decimal currentBatteryLevel)
        {
            var desiredStateOfChargeKwh = decimal.Multiply(carData.BatteryCapacity / 100M, userSettings.DesiredStateOfCharge);
            var totalChargeNeeded = decimal.Subtract(desiredStateOfChargeKwh, currentBatteryLevel);
            var totalChargeTimeHour = (double)decimal.Divide(totalChargeNeeded, carData.ChargePower);

            return TimeSpan.FromHours(totalChargeTimeHour);
        }

        private static ScheduleDto StartsImmediateChargeProcess(CarData carData, string startingTime, decimal directChargingKwh, out decimal currentBatteryLevel)
        {
            var immediateChargeNeeded = decimal.Subtract(directChargingKwh, carData.CurrentBatteryLevel);
            var immediateChargeTimeHour = (double) decimal.Round(decimal.Divide(immediateChargeNeeded, carData.ChargePower), 2, MidpointRounding.AwayFromZero);
            currentBatteryLevel = decimal.Add(carData.CurrentBatteryLevel, immediateChargeNeeded);

            var endingTime = DateTime.Parse(startingTime).AddHours(immediateChargeTimeHour);

            return new ScheduleDto(startingTime, endingTime, true);
        }

        private static IEnumerable<ScheduleDto> GenerateChargingSchedule(IEnumerable<Tariffs> tariffs, TimeSpan totalChargeTimeHour, DateTime startingTime, DateTime leavingTime)
        {
            var leavingTimeDay = leavingTime.TimeOfDay < startingTime.TimeOfDay ? startingTime.AddDays(1).Day : startingTime.Day;
            leavingTime = new DateTime(startingTime.Year, startingTime.Month, leavingTimeDay, leavingTime.Hour, leavingTime.Minute, 00);

            var sortedTariffs = InsertTimeSlotsToTariffs(tariffs, startingTime, leavingTime);
            var scheduleDtos = CalculateOptimizeCharging(sortedTariffs, totalChargeTimeHour);

            return scheduleDtos.OrderBy(s => s.StartingTime).ToList();
        }

        private static IEnumerable<Tariffs> InsertTimeSlotsToTariffs(IEnumerable<Tariffs> tariffs, DateTime startingTime, DateTime leavingTime)
        {
            var midnight = startingTime.AddDays(1).Subtract(startingTime.TimeOfDay);
            var assumptionDayTariffs = new List<Tariffs>();

            tariffs.ToList().ForEach(tariff =>
            {
                var startTimeOfDay = DateTime.Parse(tariff.StartTime);
                var startTime = new DateTime(startingTime.Year, startingTime.Month, startingTime.Day, startTimeOfDay.Hour, startTimeOfDay.Minute, 00);

                assumptionDayTariffs.Add(new Tariffs(startTime.AddDays(-1).ToString(), tariff.EndTime, tariff.EnergyPrice));
                assumptionDayTariffs.Add(new Tariffs(startTime.ToString(), tariff.EndTime, tariff.EnergyPrice));
                assumptionDayTariffs.Add(new Tariffs(startTime.AddDays(1).ToString(), tariff.EndTime, tariff.EnergyPrice));

                assumptionDayTariffs.ToList().ForEach(tariff =>
                {
                    var startTime = DateTime.Parse(tariff.StartTime);
                    var endTime = DateTime.Parse(tariff.EndTime);

                    if (startTime.TimeOfDay > endTime.TimeOfDay || endTime.TimeOfDay == midnight.TimeOfDay)
                        endTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, endTime.Hour, endTime.Minute, 00).AddDays(1);
                    else if (startTime.TimeOfDay < endTime.TimeOfDay)
                        endTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, endTime.Hour, endTime.Minute, 00);

                    if (endTime < startingTime || startTime > leavingTime)
                        assumptionDayTariffs.Remove(tariff);

                    tariff.StartTime = startTime >= startingTime ? startTime.ToString() : startingTime.ToString();
                    tariff.EndTime = endTime >= leavingTime ? leavingTime.ToString() : endTime.ToString();
                    tariff.TimeSlot = endTime.Subtract(startTime);
                });
            });

            return assumptionDayTariffs.OrderBy(t => t.EnergyPrice);
        }

        private static List<ScheduleDto> CalculateOptimizeCharging(IEnumerable<Tariffs> sortedTariffs, TimeSpan totalChargeTime)
        {
            var scheduleDtos = new List<ScheduleDto>();

            foreach (var tariff in sortedTariffs)
            {
                var startTime = DateTime.Parse(tariff.StartTime);
                var endTime = DateTime.Parse(tariff.EndTime);

                if (tariff.TimeSlot <= totalChargeTime && totalChargeTime.Ticks > 0)
                {
                    scheduleDtos.Add(new ScheduleDto(startTime, endTime, true));
                    totalChargeTime -= tariff.TimeSlot;
                }
                else if (tariff.TimeSlot > totalChargeTime && totalChargeTime.Ticks > 0)
                {
                    scheduleDtos.Add(new ScheduleDto(startTime, startTime.Add(totalChargeTime), true));
                    scheduleDtos.Add(new ScheduleDto(startTime.Add(totalChargeTime), endTime, false));
                    totalChargeTime -= totalChargeTime;
                }
                else
                    scheduleDtos.Add(new ScheduleDto(startTime, endTime, false));
            }

            return scheduleDtos;
        }

        private static List<ScheduleDto> SortByStartTime(List<ScheduleDto> scheduleDtos)
        {
            var tafiffs = scheduleDtos.ConvertAll(x => new 
            {
                StartTime = DateTime.Parse(x.StartingTime),
                EndTime = DateTime.Parse(x.EndingTime),
                x.IsCharging
            }).OrderBy(t => t.StartTime).ToList();

            var schedules = new List<ScheduleDto>();

            tafiffs.ForEach(tariff =>
            {
                var previousTariff = schedules.LastOrDefault();
                if (schedules.Any() && previousTariff.IsCharging == tariff.IsCharging)
                {
                    schedules.Add(new ScheduleDto(previousTariff.StartingTime, tariff.EndTime, tariff.IsCharging));
                    schedules.Remove(previousTariff);
                }
                else
                    schedules.Add(new ScheduleDto(tariff.StartTime, tariff.EndTime, tariff.IsCharging));
            });

            return schedules;
        }

        private static void IsValidChargeProfileSchedule(Schedule schedule)
        {
            var validator = new ScheduleValidator().Validate(schedule);

            if (!validator.IsValid)
                throw new ArgumentException(validator.ToString(", "));
        }
    }
}