using MediatR;
using TaskManager.Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");

        group.MapPost("/login", async ([FromBody] LoginCommand command, IMediator mediator) =>
        {
            try
            {
                var token = await mediator.Send(command);
                return Results.Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException)
            {
                return Results.Unauthorized();
            }
        })
        .WithName("Login")
        .AllowAnonymous();
    }
}