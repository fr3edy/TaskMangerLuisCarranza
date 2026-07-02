using MediatR;
using TaskManager.Application.Contracts;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Users.Commands;

public record RegisterUserCommand(string Email, string Password, string Name) : IRequest<Guid>;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {

        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User(
             Guid.NewGuid(),
             request.Email,
             passwordHash,
             request.Name
         );

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return newUser.Id;
    }
}