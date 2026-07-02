using Moq;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Users.Queries;
using TaskManager.Domain.Entities;
using FluentAssertions;

namespace TaskManager.Tests.Application.Features.Users.Queries;

public class GetUsersQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUsersQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Mapped_UserResponses()
    {
        // Arrange
        var usersList = new List<User>
        {
            new User(Guid.NewGuid(), "admin@empresa.mx", "hash", "Admin"),
            new User(Guid.NewGuid(), "dev@empresa.mx", "hash", "Dev")
        };

        _mockUserRepository
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(usersList);

        var query = new GetUsersQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);

        // Validamos que el mapeo de tus propiedades dinámicas funcionó
        var firstUser = result.First();
        firstUser.Role.Should().Be("Administrator");
        firstUser.Status.Should().Be("Active");
        firstUser.Name.Should().Be("admin"); // Tu lógica actual de Split('@')[0]
    }
}
