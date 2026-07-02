using Moq;
using FluentAssertions;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Tasks.Queries;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Application.Features.Tasks.Queries;

public class GetTasksQueryHandlerTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly GetTasksQueryHandler _handler;

    public GetTasksQueryHandlerTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _handler = new GetTasksQueryHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Mapped_TaskResponses()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tasks = new List<TaskItem>
        {
            new TaskItem(Guid.NewGuid(), "Tarea 1", "Desc 1", DateTime.UtcNow.AddDays(1), userId),
            new TaskItem(Guid.NewGuid(), "Tarea 2", "Desc 2", DateTime.UtcNow.AddDays(2), userId)
        };

        _mockTaskRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(tasks);

        var query = new GetTasksQuery(userId); // Ajusta si tu query requiere parámetros

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Tarea 1");
    }
}