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
        // 1. Validar si el usuario ya existe en la base de datos
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new ArgumentException("A user with this email already exists.");
        }

        // 2. Hashear la contraseña por seguridad usando BCrypt
        // Si no usas BCrypt, puedes cambiar esto por tu método de encriptación preferido
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // 3. Instanciar la entidad User (Ajusta las propiedades según tu modelo de dominio)
        var newUser = new User(
             Guid.NewGuid(),
             request.Email,
             passwordHash,
             request.Name
         // Role y Status se asignan automáticamente por defecto en el constructor
         );

        // 4. Persistir en la base de datos a través del repositorio
        await _userRepository.AddAsync(newUser, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return newUser.Id;
    }
}