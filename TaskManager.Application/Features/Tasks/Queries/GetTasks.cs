using MediatR;
using TaskManager.Application.Contracts;

namespace TaskManager.Application.Features.Tasks.Queries;

public class TaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public Guid UserId { get; set; }
}

public record GetTasksQuery(Guid UserId) : IRequest<IEnumerable<TaskResponse>>;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, IEnumerable<TaskResponse>>
{
    private readonly ITaskRepository _repository;

    public GetTasksQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TaskResponse>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _repository.GetAllAsync();

        return tasks.Select(task => new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            DueDate = task.DueDate,
            UserId = task.UserId
        });
    }
}