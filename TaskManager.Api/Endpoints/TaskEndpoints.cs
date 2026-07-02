using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Application.Features.Tasks.Queries;

namespace TaskManager.Api.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks")
                       .WithTags("Tasks")
                       .RequireAuthorization();

        static Guid? GetUserId(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdString, out var userId) ? userId : null;
        }

        // ==========================================
        // 1. LEER LAS TAREAS (GET)
        // ==========================================
        group.MapGet("/", async (ClaimsPrincipal userClaims, IMediator mediator) =>
        {
            var userId = GetUserId(userClaims);
            if (userId == null) return Results.Unauthorized();

            var query = new GetTasksQuery(userId.Value);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetTasks");

        // ==========================================
        // 2. CREAR TAREAS (POST)
        // ==========================================
        group.MapPost("/", async (CreateTaskCommand command, ClaimsPrincipal userClaims, IMediator mediator) =>
        {
            var userId = GetUserId(userClaims);
            if (userId == null) return Results.Unauthorized();

            var finalCommand = command with { UserId = userId.Value };
            var result = await mediator.Send(finalCommand);

            return Results.Created($"/api/tasks/{result.Id}", result);
        })
        .WithName("CreateTask");

        // ==========================================
        // 3. ACTUALIZAR TAREAS (PUT)
        // ==========================================
        group.MapPut("/{id:guid}", async (Guid id, [FromBody] UpdateTaskRequest request, ClaimsPrincipal userClaims, IMediator mediator) =>
        {
            var userId = GetUserId(userClaims);
            if (userId == null) return Results.Unauthorized();

            var command = new UpdateTaskCommand(
                id,
                request.Title,
                request.Description,
                request.Status,
                userId.Value
            );

            var success = await mediator.Send(command);

            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateTask");

        // ==========================================
        // 4. OBTENER UNA TAREA POR ID (GET)
        // ==========================================
        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal userClaims, IMediator mediator) =>
        {
            var userId = GetUserId(userClaims);
            if (userId == null) return Results.Unauthorized();

            var query = new GetTaskByIdQuery(id, userId.Value);
            var result = await mediator.Send(query);

            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetTaskById");

        // ==========================================
        // 5. ELIMINAR UNA TAREA (DELETE)
        // ==========================================
        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal userClaims, IMediator mediator) =>
        {
            var userId = GetUserId(userClaims);
            if (userId == null) return Results.Unauthorized();

            var command = new DeleteTaskCommand(id, userId.Value);
            var success = await mediator.Send(command);

            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTask");
    }
}
public partial class Program { }


public record UpdateTaskRequest(string Title, string Description, string Status);