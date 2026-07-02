using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Application.Contracts;

namespace TaskManager.Application.Features.Tasks.Commands;

public record TaskResponse(Guid Id, string Title, string Status);

public record CreateTaskCommand(string Title, string Description, DateTime DueDate, Guid UserId) : IRequest<TaskResponse>;

public  class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskResponse>
{
    private readonly ITaskRepository _repository;

    public CreateTaskCommandHandler(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<TaskResponse> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        if (request.DueDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("DueDate cannot be in the past.");
        }

        var task = new TaskItem(
            Guid.NewGuid(),
            request.Title,
            request.Description,
            request.DueDate,
            request.UserId
        );

        await _repository.AddAsync(task);
        await _repository.SaveChangesAsync();

        return new TaskResponse(task.Id, task.Title, task.Status.ToString());
    }
}