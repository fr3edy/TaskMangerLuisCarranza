using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public TaskItemStatus Status { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid UserId { get; private set; }

    private TaskItem() { }

    public TaskItem(Guid id, string title, string description, DateTime dueDate, Guid userId)
    {

        if (dueDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("DueDate cannot be in the past");
        }

        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required");

        Id = id;
        Title = title;
        Description = description;  
        Status = TaskItemStatus.Pending; 
        DueDate = dueDate;
        UserId = userId;
    }

    public void UpdateDetails(string title, string description, TaskItemStatus status)
    {
        Title = title;
        Description = description;
        Status = status;
    }

    public void MarkAsCompleted() => Status = TaskItemStatus.Completed;
}