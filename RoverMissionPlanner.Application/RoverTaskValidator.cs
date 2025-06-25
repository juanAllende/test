using FluentValidation;
using RoverMissionPlanner.Domain;

namespace RoverMissionPlanner.Application;

public class RoverTaskValidator : AbstractValidator<RoverTask>
{
    public RoverTaskValidator()
    {
        RuleFor(x => x.RoverName).NotEmpty();
        RuleFor(x => x.TaskType).IsInEnum();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
        RuleFor(x => x.StartsAt).NotEmpty();
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(1440);
        RuleFor(x => x.Status).IsInEnum();
    }
}
