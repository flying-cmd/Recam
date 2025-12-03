using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Remp.Common.Exceptions;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;

namespace Remp.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepositoryMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IBlobStorageService> _blobStorageServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _blobStorageServiceMock = new Mock<IBlobStorageService>();
        _authService = new AuthService(_authRepositoryMock.Object, _jwtTokenServiceMock.Object, _blobStorageServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WhenEmailNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Email = "notfound@example.com", Password = "Pass123$" };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(loginRequest.Email)).ReturnsAsync((User?)null);
    
        // Act
        var act = async () => await _authService.LoginAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(loginRequest.Email), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Email = "test@example.com", Password = "WrongPass123$" };
        var user = new User { Email = loginRequest.Email };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(loginRequest.Email)).ReturnsAsync(user);
        _authRepositoryMock.Setup(r => r.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(false);
    
        // Act
        var act = async () => await _authService.LoginAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(loginRequest.Email), Times.Once);
        _authRepositoryMock.Verify(r => r.CheckPasswordAsync(user, loginRequest.Password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsCorrect_ShouldReturnToken()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Email = "test@example.com", Password = "Pass123$" };
        var user = new User { Email = loginRequest.Email };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(loginRequest.Email)).ReturnsAsync(user);
        _authRepositoryMock.Setup(r => r.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(true);
        _jwtTokenServiceMock.Setup(j => j.CreateTokenAsync(user)).ReturnsAsync("jwt-token");
    
        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().Be("jwt-token");
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(loginRequest.Email), Times.Once);
        _authRepositoryMock.Verify(r => r.CheckPasswordAsync(user, loginRequest.Password), Times.Once);
        _jwtTokenServiceMock.Verify(j => j.CreateTokenAsync(user), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldThrowConflictException()
    {
        // Arrange
        var avatarFile = new Mock<IFormFile>();
        var registerRequest = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "password",
            FirstName = "Test",
            LastName = "Test",
            Avatar = avatarFile.Object
        };

        _blobStorageServiceMock
            .Setup(b => b.UploadFileAsync(registerRequest.Avatar))
            .ReturnsAsync("https://avatar_url");

        var user = new User { Email = registerRequest.Email, UserName = registerRequest.Email };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(registerRequest.Email)).ReturnsAsync(user);
    
        // Act
        var act = async () => await _authService.RegisterAsync(registerRequest);

        // Assert
        await act.Should().ThrowAsync<RegisterException>();
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(registerRequest.Email), Times.Once);
        _blobStorageServiceMock.Verify(b => b.UploadFileAsync(registerRequest.Avatar), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailDoesNotExist_ShouldCreateAgentAndReturnToken()
    {
        // Arrange
        var avatarFile = new Mock<IFormFile>();
        var registerRequest = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "password",
            FirstName = "Test",
            LastName = "Test",
            Avatar = avatarFile.Object
        };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(registerRequest.Email)).ReturnsAsync((User?)null);

        _blobStorageServiceMock
            .Setup(b => b.UploadFileAsync(registerRequest.Avatar))
            .ReturnsAsync("https://avatar_url");

        //var user = new User
        //{
        //    Email = registerRequest.Email,
        //    UserName = registerRequest.Email,
        //};
        //var agent = new Agent
        //{
        //    Id = Guid.NewGuid().ToString(),
        //    AgentFirstName = registerRequest.FirstName,
        //    AgentLastName = registerRequest.LastName,
        //    CompanyName = registerRequest.CompanyName,
        //    AvataUrl = registerRequest.AvatarUrl,
        //};
        //_authRepositoryMock.Setup(r => r.CreateAgentAsync(user, agent, registerRequest.Password, RoleNames.Agent)).Returns(Task.CompletedTask);
        _authRepositoryMock
            .Setup(r => r.CreateAgentAsync(
                It.Is<User>(u => 
                    u.Email == registerRequest.Email &&
                    u.UserName == registerRequest.Email),
                It.Is<Agent>(a => 
                    a.AgentFirstName == registerRequest.FirstName &&
                    a.AgentLastName == registerRequest.LastName &&
                    a.CompanyName == registerRequest.CompanyName &&
                    a.AvataUrl == "https://avatar_url"),
                registerRequest.Password,
                RoleNames.Agent))
            .Returns(Task.CompletedTask);
        
        _jwtTokenServiceMock
            .Setup(j => j.CreateTokenAsync(It.Is<User>(u => u.Email == registerRequest.Email)))
            .ReturnsAsync("jwt-token");


        // Act
        var result = await _authService.RegisterAsync(registerRequest);

        // Assert
        result.Should().Be("jwt-token");
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(registerRequest.Email), Times.Once);
        _blobStorageServiceMock.Verify(b => b.UploadFileAsync(registerRequest.Avatar), Times.Once);
        //_authRepositoryMock.Verify(r => r.CreateAgentAsync(user, agent, registerRequest.Password, RoleNames.Agent), Times.Once);
        //_jwtTokenServiceMock.Verify(j => j.CreateTokenAsync(user), Times.Once);
        _authRepositoryMock.Verify(r => r.CreateAgentAsync(
                It.Is<User>(u => 
                    u.Email == registerRequest.Email &&
                    u.UserName == registerRequest.Email),
                It.Is<Agent>(a => 
                    a.AgentFirstName == registerRequest.FirstName &&
                    a.AgentLastName == registerRequest.LastName &&
                    a.CompanyName == registerRequest.CompanyName &&
                    a.AvataUrl == "https://avatar_url"),
                registerRequest.Password,
                RoleNames.Agent),
            Times.Once);

        _jwtTokenServiceMock
            .Verify(j => j.CreateTokenAsync(
                It.Is<User>(u => u.Email == registerRequest.Email)), 
            Times.Once);
    }
}
