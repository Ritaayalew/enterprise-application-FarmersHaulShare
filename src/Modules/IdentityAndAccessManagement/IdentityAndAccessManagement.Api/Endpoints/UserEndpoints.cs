using IdentityAndAccessManagement.Application.Commands;
using IdentityAndAccessManagement.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

namespace IdentityAndAccessManagement.Api.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
                       .RequireAuthorization();

        // GET /users/{id}
        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id));
            return result is null ? Results.NotFound() : Results.Ok(result);
        });

        // POST /users/{id}/complete-onboarding
        group.MapPost("/{id:guid}/complete-onboarding",
            async (Guid id, ISender sender) =>
        {
            await sender.Send(new CompleteUserOnboardingCommand(id));
            return Results.NoContent();
        });
    }
}
