namespace TaskManager.Api.Endpoints;

public static class EndpointExtensions
{
    public static WebApplication TaskManagerEndpoints(this WebApplication app)
    {
        var apiGroup = app.MapGroup("/api");

        apiGroup.MapAuthEndpoints();
        apiGroup.MapTaskEndpoints();
        apiGroup.MapUserEndpoints();


        return app;
    }
}