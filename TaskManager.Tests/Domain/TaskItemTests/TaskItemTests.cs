using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Tests.Domain.TaskItemTests;

public class TaskItemTests
{
    [Fact]
    public void CreateTaskItem_WithPastDate_ShouldThrowException()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);

        Action action = () => new TaskItem(Guid.NewGuid(), "Valid Title", "Desc", pastDate, Guid.NewGuid());

        action.Should().Throw<ArgumentException>().WithMessage("DueDate cannot be in the past");
    }

    [Fact]
    public void MarkAsCompleted_ShouldChangeStatusToCompleted()
    {
        var task = new TaskItem(Guid.NewGuid(), "Title", "Desc", DateTime.UtcNow.AddDays(1), Guid.NewGuid());

        task.MarkAsCompleted();

        task.Status.Should().Be(TaskItemStatus.Completed);
    }
}