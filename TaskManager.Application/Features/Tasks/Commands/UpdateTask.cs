using MediatR;
using TaskManager.Application.Contracts;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Commands;

public record UpdateTaskCommand(Guid Id,string Title,string Description, string Status,Guid userid) : IRequest<bool>;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly ITaskRepository _repository;

    public UpdateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id);

        if (task == null) return false;

        task.UpdateDetails(request.Title, request.Description, Enum.Parse<TaskItemStatus>(request.Status));

        await _repository.UpdateAsync(task);
        await _repository.SaveChangesAsync();

        return true;
    }
}