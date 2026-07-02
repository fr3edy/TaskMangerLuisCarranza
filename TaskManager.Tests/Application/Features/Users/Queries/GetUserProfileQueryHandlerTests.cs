using Moq;
using TaskManager.Application.Contracts;
using TaskManager.Application.Features.Users.Queries;
using TaskManager.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace TaskManager.Tests.Application.Features.Users.Queries;

public class GetUserProfileQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly GetUserProfileQueryHandler _handler;

    public GetUserProfileQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _handler = new GetUserProfileQueryHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_UserProfileResponse_When_User_Exists()
    {
        // Arrange
        var query = new GetUserProfileQuery("developer@software.com");
        var user = new User(Guid.NewGuid(), query.Email, "hashedPass", "Dev User");

        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(query.Email);
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be("developer"); // Valida tu lógica de Split('@')[0]
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_User_Does_Not_Exist()
    {
        // Arrange
        var query = new GetUserProfileQuery("ghost@software.com");

        _mockUserRepository
            .Setup(repo => repo.GetUserByEmailAsync(query.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}