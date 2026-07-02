using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class TaskEndpointsTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly SqliteConnection _connection;

    public TaskEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });
            });
        });

        _client = _factory.CreateClient();
    }

    void IDisposable.Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    private string GenerateTestJwtToken(Guid userId)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EstaEsUnaClaveSuperSecretaParaElTestTecnico123!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task GetTasks_Without_Token_Should_Return_Unauthorized()
    {
        var response = await _client.GetAsync("/api/tasks/");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateTask_With_Valid_Token_Should_Return_Created()
    {
        var userId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            var testUser = new User(userId, "test@integrations.com", "hashFalso", "Test User");
            db.Users.Add(testUser);
            await db.SaveChangesAsync();
        }

        var token = GenerateTestJwtToken(userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var command = new { Title = "Task from Integration Test", Description = "Desc", DueDate = DateTime.UtcNow.AddDays(5) };

        var response = await _client.PostAsJsonAsync("/api/tasks", command); // Ajusta la URL si es necesario

        // 💡 TIP: Vuelve a agregar esta línea, es oro puro si te vuelve a dar un 500, 
        // ya que te imprimirá el error exacto de tu API en la consola de xUnit.
        var responseString = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created, $"Error del servidor: {responseString}");
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("task");
    }
}