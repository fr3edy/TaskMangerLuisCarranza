using FluentAssertions;
using Moq;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Tasks.Commands;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Application.Features.Tasks.Commands;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
        _handler = new CreateTaskCommandHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Task_When_DueDate_Is_In_Future()
    {
        // Arrange
        var command = new CreateTaskCommand("Nueva Tarea", "Descripción", DateTime.UtcNow.AddDays(5), Guid.NewGuid());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(command.Title);

        // Verificamos que se llamó al repositorio exactamente como lo pide tu interfaz (sin CancellationToken)
        _mockTaskRepository.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Once);
        _mockTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_ArgumentException_When_DueDate_Is_In_Past()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var command = new CreateTaskCommand("Tarea Atrasada", "Descripción", pastDate, Guid.NewGuid());

        // Act & Assert
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("DueDate cannot be in the past.");

        // Validamos que por seguridad, NUNCA se intente guardar en BD
        _mockTaskRepository.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Never);
        _mockTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}