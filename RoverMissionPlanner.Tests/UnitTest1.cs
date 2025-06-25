using RoverMissionPlanner.Application;
using RoverMissionPlanner.Domain;

namespace RoverMissionPlanner.Tests;

public class RoverTaskServiceTests
{
    [Fact]
    public async Task CreateTaskAsync_Should_Add_Task_When_No_Overlap()
    {
        var service = new RoverTaskService();
        var task = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover1",
            TaskType = TaskType.Drill,
            Latitude = 0,
            Longitude = 0,
            StartsAt = DateTime.UtcNow.Date.AddHours(8),
            DurationMinutes = 60,
            Status = Domain.TaskStatus.Planned
        };
        var result = await service.CreateTaskAsync(task);
        Assert.Equal(task.Id, result.Id);
    }

    [Fact]
    public async Task CreateTaskAsync_Should_Throw_When_Overlap()
    {
        var service = new RoverTaskService();
        var baseTask = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover1",
            TaskType = TaskType.Drill,
            Latitude = 0,
            Longitude = 0,
            StartsAt = DateTime.UtcNow.Date.AddHours(8),
            DurationMinutes = 60,
            Status = Domain.TaskStatus.Planned
        };
        await service.CreateTaskAsync(baseTask);
        var overlappingTask = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover1",
            TaskType = TaskType.Sample,
            Latitude = 0,
            Longitude = 0,
            StartsAt = DateTime.UtcNow.Date.AddHours(8).AddMinutes(30),
            DurationMinutes = 60,
            Status = Domain.TaskStatus.Planned
        };
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateTaskAsync(overlappingTask));
    }

    [Fact]
    public async Task GetUtilizationForRoverOnDateAsync_Should_Return_Correct_Percentage()
    {
        var service = new RoverTaskService();
        var task = new RoverTask
        {
            Id = Guid.NewGuid(),
            RoverName = "Rover1",
            TaskType = TaskType.Drill,
            Latitude = 0,
            Longitude = 0,
            StartsAt = DateTime.UtcNow.Date.AddHours(8),
            DurationMinutes = 120,
            Status = Domain.TaskStatus.Planned
        };
        await service.CreateTaskAsync(task);
        var utilization = await service.GetUtilizationForRoverOnDateAsync("Rover1", DateTime.UtcNow.Date);
        Assert.True(utilization > 0 && utilization <= 1);
    }
}
