using RoverMissionPlanner.Domain;

namespace RoverMissionPlanner.Application;

public interface IRoverTaskService
{
    Task<RoverTask> CreateTaskAsync(RoverTask task);
    Task<List<RoverTask>> GetTasksForRoverOnDateAsync(string roverName, DateTime date);
    Task<double> GetUtilizationForRoverOnDateAsync(string roverName, DateTime date);
}

public class RoverTaskService : IRoverTaskService
{
    private readonly List<RoverTask> _tasks = new();

    public Task<RoverTask> CreateTaskAsync(RoverTask task)
    {
        // Validar solapamiento
        var overlap = _tasks.Any(t => t.RoverName == task.RoverName &&
            t.StartsAt.Date == task.StartsAt.Date &&
            t.Id != task.Id &&
            t.Status != Domain.TaskStatus.Aborted &&
            ((task.StartsAt < t.StartsAt.AddMinutes(t.DurationMinutes)) &&
             (t.StartsAt < task.StartsAt.AddMinutes(task.DurationMinutes))));
        if (overlap)
            throw new InvalidOperationException("Task overlaps with another task for this rover.");
        _tasks.Add(task);
        return Task.FromResult(task);
    }

    public Task<List<RoverTask>> GetTasksForRoverOnDateAsync(string roverName, DateTime date)
    {
        var result = _tasks
            .Where(t => t.RoverName == roverName && t.StartsAt.Date == date.Date)
            .OrderBy(t => t.StartsAt)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<double> GetUtilizationForRoverOnDateAsync(string roverName, DateTime date)
    {
        var totalMinutes = _tasks
            .Where(t => t.RoverName == roverName && t.StartsAt.Date == date.Date && t.Status != Domain.TaskStatus.Aborted)
            .Sum(t => t.DurationMinutes);
        double utilization = totalMinutes / 1440.0; // 1440 minutos en un día
        return Task.FromResult(utilization);
    }
}
