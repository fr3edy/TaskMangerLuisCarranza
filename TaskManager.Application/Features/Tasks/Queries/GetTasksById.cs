using MediatR;
using TaskManager.Application.Contracts;

namespace TaskManager.Application.Features.Tasks.Queries;

public record GetTaskByIdQuery(Guid Id, Guid UserId) : IRequest<TaskResponse?>;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskResponse?>
{
    private readonly ITaskRepository _repository;

    public GetTaskByIdQueryHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskResponse?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id);

        if (task == null) return null;

        return new TaskResponse
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            DueDate = task.DueDate,
            UserId = task.UserId
        };
    }
}