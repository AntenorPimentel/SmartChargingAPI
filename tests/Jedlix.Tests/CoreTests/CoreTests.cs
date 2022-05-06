using Jedlix.Core;
using Jedlix.Core.Models;
using Jedlix.Tests.Extensions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Jedlix.Tests
{
    public class CoreTests
    {
        private readonly ScheduleService _sut;

        public CoreTests()
        {
            _sut = new ScheduleService();
        }

        [Fact]
        public async Task Given_Schedule_WithInvalidStartingTime_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                .WithInvalidStartingTime()
                .WithUserSettings()
                .WithCarData();

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("StartingTime is required", ex.Message);
        }

        [Fact]
        public async Task Given_Schedule_WithoutLeavingTime_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                .WithStartingTime("2022-02-07T22:12:23Z")
                .WithUserSettings().WithInvalidLeavingTime()
                .WithCarData();

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("LeavingTime is required", ex.Message);
        }

        [Fact]
        public async Task Given_Schedule_WithoutTariffs_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                .WithStartingTime("2022-02-07T22:12:23Z")
                .WithUserSettings().WithoutTariffs()
                .WithCarData();

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("Tariffs are required", ex.Message);
        }

        [Fact]
        public async Task Given_Schedule_WithoutTariffsStartTime_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                .WithStartingTime("2022-02-07T22:12:23Z")
                .WithUserSettings().WithInvalidTariff(new Tariffs().Build().WithInvalidStartTime())
                .WithCarData();

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("StartTime is required", ex.Message);
        }

        [Fact]
        public async Task Given_Schedule_WithoutTariffsEndTime_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                .WithStartingTime("2022-02-07T22:12:23Z")
                .WithUserSettings().WithInvalidTariff(new Tariffs().Build().WithInvalidEndTime())
                .WithCarData();

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("EndTime is required", ex.Message);
        }

        [Fact]
        public async Task Given_Schedule_WithoutChargePower_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                 .WithStartingTime("2022-02-07T22:12:23Z")
                 .WithUserSettings()
                 .WithInvalidCarData(new CarData().Build().WithInvalidChargePower());

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("ChargePower is required", ex.Message);
        }

        [Fact]
        public async Task Given_Schedule_WithoutBatteryCapacity_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                 .WithStartingTime("2022-02-07T22:12:23Z")
                 .WithUserSettings()
                 .WithInvalidCarData(new CarData().Build().WithInvalidBatteryCapacity());

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("BatteryCapacity is required", ex.Message);
        }

        [Fact]
        public async Task Given_Schedule_WithoutCurrentBatteryLevel_Then_ThrowException()
        {
            var schedule = new Schedule().Build()
                 .WithStartingTime("2022-02-07T22:12:23Z")
                 .WithUserSettings()
                 .WithInvalidCarData(new CarData().Build().WithInvalidCurrentBatteryLevel());

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CalculateOptimalSchedule(schedule));

            Assert.Equal("CurrentBatteryLevel is required", ex.Message);
        }
    }
}