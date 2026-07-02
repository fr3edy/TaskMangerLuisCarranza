using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    // Inyectamos ILogger preparándonos para la siguiente fase de Logging
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Registramos el error en los logs
        _logger.LogError(exception, "Ocurrió una excepción no controlada: {Message}", exception.Message);

        // 2. Mapeamos el tipo de excepción a un código HTTP adecuado
        var (statusCode, title) = exception switch
        {
            // Si es un error de validación de nuestro Dominio (ej. fecha en el pasado)
            ArgumentException => (StatusCodes.Status400BadRequest, "Error de Validación"),

            // Si es cualquier otro error no controlado
            _ => (StatusCodes.Status500InternalServerError, "Error Interno del Servidor")
        };

        // 3. Estructuramos la respuesta usando el estándar ProblemDetails
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Retornamos true para indicarle al framework que ya manejamos la excepción
        return true;
    }
}
