using FluentValidation;
using Jedlix.Core.Models;

namespace Jedlix.Core.Validators
{
    public class ScheduleValidator : AbstractValidator<Schedule>
    {
        public ScheduleValidator()
        {
            RuleFor(x => x.StartingTime).NotNull().NotEmpty().WithMessage("StartingTime is required");
            RuleFor(x => x.UserSettings).SetValidator(new UserSettingsValidator());
            RuleFor(x => x.CarData).SetValidator(new CarDataValidator());
        }
    }

    public class UserSettingsValidator : AbstractValidator<UserSettings>
    {
        public UserSettingsValidator()
        {
            RuleFor(x => x.LeavingTime).NotNull().NotEmpty().WithMessage("LeavingTime is required");
            RuleFor(x => x.Tariffs.Count()).NotEqual(0).WithMessage("Tariffs are required");
            RuleForEach(x => x.Tariffs).SetValidator(new TariffsValidator());
        }
    }

    public class TariffsValidator : AbstractValidator<Tariffs>
    {
        public TariffsValidator()
        {
            RuleFor(x => x.StartTime).NotNull().NotEmpty().WithMessage("StartTime is required");
            RuleFor(x => x.EndTime).NotNull().NotEmpty().WithMessage("EndTime is required");
            RuleFor(x => x.EnergyPrice).NotNull().NotEmpty().WithMessage("EnergyPrice is required");
        }
    }

    public class CarDataValidator : AbstractValidator<CarData>
    {
        public CarDataValidator()
        {
            RuleFor(x => x.ChargePower).NotNull().GreaterThan(0).WithMessage("ChargePower is required");
            RuleFor(x => x.CurrentBatteryLevel).NotNull().GreaterThan(0).WithMessage("CurrentBatteryLevel is required");
            RuleFor(x => x.BatteryCapacity).NotNull().GreaterThan(0).WithMessage("BatteryCapacity is required");
        }
    }
}
