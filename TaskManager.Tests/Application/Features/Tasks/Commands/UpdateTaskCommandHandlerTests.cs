using Moq;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Entities;
using FluentAssertions;

namespace TaskManager.Tests.Application.Features.Tasks.Commands;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly UpdateTaskCommandHandler _handler;

    public UpdateTaskCommandHandlerTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _handler = new UpdateTaskCommandHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_True_And_Update_When_Task_Exists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TaskItem(taskId, "Viejo Título", "Vieja Desc", DateTime.UtcNow.AddDays(1), Guid.NewGuid());
        var command = new UpdateTaskCommand(taskId, "Nuevo Título", "Nueva Desc", "Completed", Guid.NewGuid());

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        existingTask.Title.Should().Be("Nuevo Título");

        _mockTaskRepository.Verify(repo => repo.UpdateAsync(existingTask), Times.Once);
        _mockTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Task_Does_Not_Exist()
    {
        // Arrange
        var command = new UpdateTaskCommand(Guid.NewGuid(), "Titulo", "Desc", "Completed", Guid.NewGuid());

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();

        _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);
        _mockTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}