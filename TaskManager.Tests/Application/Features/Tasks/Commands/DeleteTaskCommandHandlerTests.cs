using Moq;
using FluentAssertions;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Application.Features.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _handler = new DeleteTaskCommandHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_True_And_Delete_When_Task_Exists()
    {
        // Arrange
        var command = new DeleteTaskCommand(Guid.NewGuid(), Guid.NewGuid());
        var existingTask = new TaskItem(command.Id, "Titulo", "Desc", DateTime.UtcNow, Guid.NewGuid());

        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync(existingTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _mockTaskRepository.Verify(repo => repo.DeleteAsync(existingTask), Times.Once);
        _mockTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_False_When_Task_Does_Not_Exist()
    {
        // Arrange
        var command = new DeleteTaskCommand(Guid.NewGuid(), Guid.NewGuid());
        _mockTaskRepository.Setup(repo => repo.GetByIdAsync(command.Id)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        _mockTaskRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TaskItem>()), Times.Never);
        _mockTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}