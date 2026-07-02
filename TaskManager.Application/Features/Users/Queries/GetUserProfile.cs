using MediatR;
using TaskManager.Application.Contracts;

namespace TaskManager.Application.Features.Users.Queries;

public record GetUserProfileQuery(string Email) : IRequest<UserProfileResponse?>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse?>
{
    private readonly IUserRepository _repository;

    public GetUserProfileQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileResponse?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetUserByEmailAsync(request.Email, cancellationToken);

        if (user == null) return null;

        return new UserProfileResponse
        {
            Id = user.Id,
            Name = user.Email.Split('@')[0],
            Email = user.Email,
            JoinDate = DateTime.UtcNow.AddMonths(-24) 
        };
    }
}