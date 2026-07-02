namespace TaskManager.Api.Endpoints;

public static class EndpointExtensions
{
    public static WebApplication TaskManagerEndpoints(this WebApplication app)
    {
        // Creamos un grupo base para la API versión 1
        var apiGroup = app.MapGroup("/api");

        // Aquí irás registrando tus módulos de Minimal APIs
        apiGroup.MapAuthEndpoints();
        apiGroup.MapTaskEndpoints();
        apiGroup.MapUserEndpoints();


        return app;
    }
}