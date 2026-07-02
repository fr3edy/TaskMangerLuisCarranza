using Moq;
using FluentAssertions;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Tasks.Queries;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Application.Features.Tasks.Queries;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly GetTaskByIdQueryHandler _handler;

    public GetTaskByIdQueryHandlerTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _handler = new GetTaskByIdQueryHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_TaskResponse_When_Task_Exists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var task = new TaskItem(taskId, "Mi Tarea", "Desc", DateTime.UtcNow.AddDays(1), userId);
        var query = new GetTaskByIdQuery(taskId, userId);

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(task);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be("Mi Tarea");
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Task_Does_Not_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new GetTaskByIdQuery(Guid.NewGuid(), userId);
        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(query.Id)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}