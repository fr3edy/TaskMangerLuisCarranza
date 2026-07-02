using FluentAssertions;
using Moq;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Auth.Commands;
using TaskManager.Domain.Entities;

namespace TaskManager.Tests.Application.Features.Auth.Commands;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IJwtProvider> _mockJwtProvider;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockJwtProvider = new Mock<IJwtProvider>();
        _handler = new LoginCommandHandler(_mockUserRepository.Object, _mockJwtProvider.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Token_When_Credentials_Are_Valid()
    {
        // ==========================================
        // ARRANGE (Camino Feliz)
        // ==========================================
        var command = new LoginCommand("admin@empresa.com", "Password123!");

        // Generamos un hash real usando BCrypt para que la validación interna del Handler pase
        string realHash = BCrypt.Net.BCrypt.HashPassword(command.Password);
        var user = new User(Guid.NewGuid(), command.Email, realHash, "Admin");

        // Simulamos que el repositorio encuentra al usuario
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Simulamos que el proveedor JWT genera un token
        var expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.MockedToken";
        _mockJwtProvider
            .Setup(jwt => jwt.Generate(user))
            .Returns(expectedToken);

        // ==========================================
        // ACT
        // ==========================================
        var result = await _handler.Handle(command, CancellationToken.None);

        // ==========================================
        // ASSERT
        // ==========================================
        result.Should().Be(expectedToken);

        // Verificamos que se haya intentado generar el token exactamente una vez
        _mockJwtProvider.Verify(jwt => jwt.Generate(user), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedAccessException_When_User_Not_Found()
    {
        // ==========================================
        // ARRANGE (Camino Triste 1: No existe el correo)
        // ==========================================
        var command = new LoginCommand("fantasma@empresa.com", "Password123!");

        // Simulamos que el repositorio no encuentra nada
        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // ==========================================
        // ACT & ASSERT
        // ==========================================
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");

        // Seguridad: Si no hay usuario, jamás debe intentar generar un token
        _mockJwtProvider.Verify(jwt => jwt.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_UnauthorizedAccessException_When_Password_Is_Invalid()
    {
        // ==========================================
        // ARRANGE (Camino Triste 2: Contraseña incorrecta)
        // ==========================================
        var command = new LoginCommand("admin@empresa.com", "ContraseñaEquivocada!");

        // Guardamos una contraseña distinta a la que viene en el command
        string realHash = BCrypt.Net.BCrypt.HashPassword("PasswordCorrecta123!");
        var user = new User(Guid.NewGuid(), command.Email, realHash, "Admin");

        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // ==========================================
        // ACT & ASSERT
        // ==========================================
        Func<Task> action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Credenciales inválidas.");

        // Seguridad: Si la contraseña es mala, jamás debe intentar generar un token
        _mockJwtProvider.Verify(jwt => jwt.Generate(It.IsAny<User>()), Times.Never);
    }
}