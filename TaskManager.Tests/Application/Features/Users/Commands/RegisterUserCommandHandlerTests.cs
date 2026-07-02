using FluentAssertions;
using Moq;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Users.Commands;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Application.Features.Users.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new RegisterUserCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_User_When_Email_Is_Unique()
    {
        // ==========================================
        // ARRANGE (Preparar el Camino Feliz)
        // ==========================================
        var command = new RegisterUserCommand("alfredo@carranza.com", "Password123!", "Alfredo Carranza");

        // Simulamos que al buscar en la BD, no se encuentra el correo (retorna null)
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // ==========================================
        // ACT (Actuar)
        // ==========================================
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // ==========================================
        // ASSERT (Afirmar)
        // ==========================================
        resultId.Should().NotBeEmpty();

        // Verificamos que se haya llamado al AddAsync con los datos correctos
        // Y muy importante: Verificamos que el PasswordHash NO sea la contraseña en texto plano
        _mockUserRepository.Verify(repo => repo.AddAsync(
            It.Is<User>(u =>
                u.Email == command.Email &&
                u.Name == command.Name &&
                u.PasswordHash != command.Password), // 👈 Valida que hubo encriptación
            It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Email_Already_Exists()
    {
        // ==========================================
        // ARRANGE (Preparar el Camino Triste)
        // ==========================================
        var command = new RegisterUserCommand("alfredo@carranza.com", "Password123!", "Alfredo Carranza");
        var existingUser = new User(Guid.NewGuid(), command.Email, "algonhash", "Alfredo Carranza");

        // Simulamos que el correo YA existe en la base de datos
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);

        // ==========================================
        // ACT & ASSERT
        // ==========================================

        // Atrapamos la ejecución en una función para validar que explote
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        // Validamos que arroje ArgumentException con el mensaje exacto
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("A user with this email already exists.");

        // Validamos la SEGURIDAD: Nunca debió intentar guardar en la base de datos
        _mockUserRepository.Verify(repo => repo.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUserRepository.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}