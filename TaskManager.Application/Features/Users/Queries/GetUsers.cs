using MediatR;
using TaskManager.Application.Contracts;


namespace TaskManager.Application.Features.Users.Queries;

public record GetUsersQuery : IRequest<IEnumerable<UserResponse>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUserRepository _repository;

    public GetUsersQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repository.GetAllAsync();

        return users.Select(u => new UserResponse
        {
            Id = u.Id,
            Email = u.Email,
            // Asumiendo que tu entidad tiene una propiedad Name, si no, puedes poner u.Email temporalmente
            Name = u.Email.Split('@')[0],
            Role = "Administrator",
            Status = "Active"
        });
    }
}