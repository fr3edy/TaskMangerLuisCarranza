using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using TaskManager.Api.Endpoints;
using TaskManager.Api.Middleware;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CONFIGURACIÓN DE LOGS (Serilog)
// ==========================================
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "Logs/TaskManager-log-.txt",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        );
});

// ==========================================
// 2. REGISTRO DE SERVICIOS (Inyección de Dependencias)
// ==========================================
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly));

// ==========================================
// 3. CONFIGURACIÓN DE CORS (¡Corregido para Angular!)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        // 🔴 CORRECCIÓN CRÍTICA: Aquí debe ir el puerto de Angular (4200), NO el de .NET
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ==========================================
// 4. CONFIGURACIÓN DE SEGURIDAD (JWT)
// ==========================================
var jwtKey = builder.Configuration["Jwt:SecretKey"] ?? "EstaEsUnaClaveSuperSecretaParaElTestTecnico123!";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ==========================================
// 5. UTILIDADES DE API (Swagger, Manejo de Errores)
// ==========================================
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// =======================================================================
// CONSTRUCCIÓN DE LA APLICACIÓN
// =======================================================================
var app = builder.Build();

// ==========================================
// 6. INICIALIZACIÓN DE DATOS (Data Seeder)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DataSeeder.Initialize(context);
}

// ==========================================
// 7. PIPELINE HTTP (Middlewares - EL ORDEN ES VITAL)
// ==========================================

// A. Interceptor global de errores (Debe ir primero para atrapar todo)
app.UseExceptionHandler();

// B. Documentación de API (Solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// C. Seguridad de red
app.UseHttpsRedirection();

// D. CORS (Tiene que ir ANTES de Autenticación/Autorización)
app.UseCors("AllowAngularApp");

// E. Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

// ==========================================
// 8. MAPEO DE ENDPOINTS Y ARRANQUE
// ==========================================
app.TaskManagerEndpoints();

app.Run();