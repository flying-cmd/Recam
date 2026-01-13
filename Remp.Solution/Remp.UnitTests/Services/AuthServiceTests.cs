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
    private readonly Mock<ILoggerService> _loggerServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepositoryMock = new Mock<IAuthRepository>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _loggerServiceMock = new Mock<ILoggerService>();
        _authService = new AuthService(
            _authRepositoryMock.Object, 
            _jwtTokenServiceMock.Object,
            _loggerServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WhenEmailNotFound_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Email = "notfound@example.com", Password = "Pass123$" };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(loginRequest.Email)).ReturnsAsync((User?)null);
        _loggerServiceMock.Setup(l => l.LogLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

        // Act
        var act = async () => await _authService.LoginAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(loginRequest.Email), Times.Once);
        _loggerServiceMock.Verify(l => l.LogLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsIncorrect_ShouldThrowUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequestDto { Email = "test@example.com", Password = "WrongPass123$" };
        var user = new User { Email = loginRequest.Email };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(loginRequest.Email)).ReturnsAsync(user);
        _authRepositoryMock.Setup(r => r.CheckPasswordAsync(user, loginRequest.Password)).ReturnsAsync(false);
        _loggerServiceMock.Setup(l => l.LogLogin("test@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    
        // Act
        var act = async () => await _authService.LoginAsync(loginRequest);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedException>();
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(loginRequest.Email), Times.Once);
        _authRepositoryMock.Verify(r => r.CheckPasswordAsync(user, loginRequest.Password), Times.Once);
        _loggerServiceMock.Verify(l => l.LogLogin("test@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
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
        _loggerServiceMock.Setup(l => l.LogLogin("test@example.com", It.IsAny<string>(), It.IsAny<string>(), null, null));
    
        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().Be("jwt-token");
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(loginRequest.Email), Times.Once);
        _authRepositoryMock.Verify(r => r.CheckPasswordAsync(user, loginRequest.Password), Times.Once);
        _jwtTokenServiceMock.Verify(j => j.CreateTokenAsync(user), Times.Once);
        _loggerServiceMock.Verify(l => l.LogLogin("test@example.com", It.IsAny<string>(), It.IsAny<string>(), null, null), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldThrowArgumentErrorException()
    {
        // Arrange
        var avatarFile = new Mock<IFormFile>();
        var registerRequest = new RegisterRequestDto
        {
            PhotographyCompanyName = "Test Photography Company",
            Email = "test@example.com",
            PhoneNumber = "123456789",
            Password = "password",
            ConfirmPassword = "password",
        };

        var user = new User { Email = registerRequest.Email, UserName = registerRequest.Email };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(registerRequest.Email)).ReturnsAsync(user);
        _loggerServiceMock.Setup(l => l.LogRegister(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    
        // Act
        var act = async () => await _authService.RegisterAsync(registerRequest);

        // Assert
        await act.Should().ThrowAsync<ArgumentErrorException>();
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(registerRequest.Email), Times.Once);
        _loggerServiceMock.Verify(l => l.LogRegister(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailDoesNotExist_ShouldCreateAgentAndReturnToken()
    {
        // Arrange
        var avatarFile = new Mock<IFormFile>();
        var registerRequest = new RegisterRequestDto
        {
            PhotographyCompanyName = "Test Photography Company",
            Email = "test@example.com",
            PhoneNumber = "123456789",
            Password = "password",
            ConfirmPassword = "password",
        };
        _authRepositoryMock.Setup(r => r.FindByEmailAsync(registerRequest.Email)).ReturnsAsync((User?)null);

        _loggerServiceMock.Setup(l => l.LogRegister("test@example.com", It.IsAny<string>(), It.IsAny<string>(), null, null));

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
                It.Is<PhotographyCompany>(pc => 
                    pc.PhotographyCompanyName == registerRequest.PhotographyCompanyName),
                registerRequest.Password,
                RoleNames.PhotographyCompany))
            .Returns(Task.CompletedTask);
        
        _jwtTokenServiceMock
            .Setup(j => j.CreateTokenAsync(It.Is<User>(u => u.Email == registerRequest.Email)))
            .ReturnsAsync("jwt-token");


        // Act
        var result = await _authService.RegisterAsync(registerRequest);

        // Assert
        result.Should().Be("jwt-token");
        _authRepositoryMock.Verify(r => r.FindByEmailAsync(registerRequest.Email), Times.Once);
        //_authRepositoryMock.Verify(r => r.CreateAgentAsync(user, agent, registerRequest.Password, RoleNames.Agent), Times.Once);
        //_jwtTokenServiceMock.Verify(j => j.CreateTokenAsync(user), Times.Once);
        _authRepositoryMock.Verify(r => r.CreateAgentAsync(
                It.Is<User>(u => 
                    u.Email == registerRequest.Email &&
                    u.UserName == registerRequest.Email),
                It.Is<PhotographyCompany>(pc => 
                    pc.PhotographyCompanyName == registerRequest.PhotographyCompanyName),
                registerRequest.Password,
                RoleNames.PhotographyCompany),
            Times.Once);

        _jwtTokenServiceMock
            .Verify(j => j.CreateTokenAsync(
                It.Is<User>(u => u.Email == registerRequest.Email)), 
            Times.Once);

        _loggerServiceMock.Verify(l => l.LogRegister("test@example.com", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}
