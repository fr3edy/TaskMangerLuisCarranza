
using MediatR;
using TaskManager.Application.Contracts;

namespace TaskManager.Application.Features.Tasks.Commands;

public record DeleteTaskCommand(Guid Id, Guid UserId) : IRequest<bool>;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _repository;
    public DeleteTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }
    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _repository.GetByIdAsync(request.Id);
        if (task == null) return false;
        await _repository.DeleteAsync(task);
        await _repository.SaveChangesAsync();
        return true;
    }
}