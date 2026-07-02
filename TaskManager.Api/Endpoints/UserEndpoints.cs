using MediatR;
using System.Security.Claims;
using TaskManager.Application.Features.Users.Commands;
using TaskManager.Application.Features.Users.Queries;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        group.MapGet("/", async (IMediator mediator) =>
        {
            var query = new GetUsersQuery();
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetUsers")
        .RequireAuthorization();

        group.MapGet("/profile", async (ClaimsPrincipal userClaims, IMediator mediator) =>
        {
            // Extrae el correo del token JWT del usuario logueado
            var email = userClaims.FindFirstValue(ClaimTypes.Email) ?? userClaims.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(email)) return Results.Unauthorized();

            var query = new GetUserProfileQuery(email);
            var result = await mediator.Send(query);

            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetUserProfile")
        .RequireAuthorization();

        group.MapPost("/register", async ([FromBody] RegisterUserCommand command, IMediator mediator) =>
        {
            try
            {
                var userId = await mediator.Send(command);
                return Results.Created($"/api/users/{userId}", new { Id = userId });
            }
            catch (ArgumentException ex)
            {
                // Si el correo ya existe, devolvemos un 400 Bad Request con el mensaje limpio
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("RegisterUser");
    }
}