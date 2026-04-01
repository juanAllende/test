using Microsoft.AspNetCore.Mvc;
using RoverMissionPlanner.Application;
using RoverMissionPlanner.Domain;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.Results;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IRoverTaskService, RoverTaskService>();
builder.Services.AddSingleton<IValidator<RoverTask>, RoverTaskValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Rover Mission Planner API", Version = "v1" });
});

var app = builder.Build();

// Middleware global de manejo de excepciones
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (ValidationException ex)
    {
        context.Response.StatusCode = 400;
        await context.Response.WriteAsJsonAsync(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

// POST /rovers/{id}/tasks
app.MapPost("/rovers/{id}/tasks", async ([FromRoute] string id, [FromBody] RoverTask task, IRoverTaskService service, IValidator<RoverTask> validator) =>
{
    ValidationResult result = await validator.ValidateAsync(task);
    if (!result.IsValid)
        return Results.BadRequest(new { errors = result.Errors.Select(e => e.ErrorMessage) });
    try
    {
        task.RoverName = id;
        var created = await service.CreateTaskAsync(task);
        return Results.Created($"/rovers/{id}/tasks/{created.Id}", created);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(new { error = ex.Message });
    }
});

// GET /rovers/{id}/tasks?date=YYYY-MM-DD
app.MapGet("/rovers/{id}/tasks", async ([FromRoute] string id, [FromQuery] DateTime date, IRoverTaskService service) =>
{
    var tasks = await service.GetTasksForRoverOnDateAsync(id, date);
    return Results.Ok(tasks);
});

// GET /rovers/{id}/utilization?date=YYYY-MM-DD
app.MapGet("/rovers/{id}/utilization", async ([FromRoute] string id, [FromQuery] DateTime date, IRoverTaskService service) =>
{
    var utilization = await service.GetUtilizationForRoverOnDateAsync(id, date);
    return Results.Ok(new { utilization });
});

app.Run();
