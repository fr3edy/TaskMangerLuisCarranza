using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;
using FluentAssertions;

namespace TaskManager.Tests.Infrastructure.Repositories;

public class TaskRepositoryTests
{
    // Función auxiliar para obtener un DbContext fresco en cada prueba
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nombre único por prueba
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAsync_Should_Save_Task_To_Database()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new TaskRepository(context);

        var userId = Guid.NewGuid();
        var task = new TaskItem(Guid.NewGuid(), "Integration Task", "Test Desc", DateTime.UtcNow.AddDays(1), userId);

        // Act
        await repository.AddAsync(task);
        await repository.SaveChangesAsync();

        // Assert
        var savedTask = await context.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);
        savedTask.Should().NotBeNull();
        savedTask!.Title.Should().Be("Integration Task");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Task_When_Exists()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var repository = new TaskRepository(context);

        var taskId = Guid.NewGuid();
        var task = new TaskItem(taskId, "Existing Task", "Desc", DateTime.UtcNow.AddDays(1), Guid.NewGuid());

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
    }
}