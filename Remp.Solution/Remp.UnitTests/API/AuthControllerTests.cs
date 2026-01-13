using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Remp.API.Controllers;
using Remp.Common.Helpers.ApiResponse;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.UnitTests.API;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "test@example.com",
            Password = "password"
        };

        _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync("jwt-token");

        // Act
        var result = await _authController.Login(request);

        // Assert
        var okResult = result.Result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value as PostResponse<string>;
        response!.Success.Should().BeTrue();
        response.Data.Should().Be("jwt-token");
        response.Message.Should().Be("Login successfully");
        _authServiceMock.Verify(a => a.LoginAsync(request), Times.Once);
    }


    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldReturn201()
    {
        // Arrange
        var avatarFile = new Mock<IFormFile>();
        var request = new RegisterRequestDto
        {
            PhotographyCompanyName = "Test Photography Company",
            Email = "test@example.com",
            PhoneNumber = "123456789",
            Password = "password",
            ConfirmPassword = "password",
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(
                It.Is<RegisterRequestDto>(
                    u => u.PhotographyCompanyName == request.PhotographyCompanyName &&
                    u.Email == request.Email &&
                    u.Password == request.Password &&
                    u.ConfirmPassword == request.ConfirmPassword &&
                    u.PhoneNumber == request.PhoneNumber
                    )
                )
            )
            .ReturnsAsync("jwt-token");
    
        // Act
        var result = await _authController.Register(request);
    
        // Assert
        var okResult = result.Result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(201);
    
        var response = okResult.Value as PostResponse<string>;
        response!.Success.Should().BeTrue();
        response.Data.Should().Be("jwt-token");
        response.Message.Should().Be("Registered successfully");

        _authServiceMock
            .Verify(a => a.RegisterAsync(
                It.Is<RegisterRequestDto>(
                    u => u.PhotographyCompanyName == request.PhotographyCompanyName &&
                    u.Email == request.Email &&
                    u.Password == request.Password &&
                    u.ConfirmPassword == request.ConfirmPassword &&
                    u.PhoneNumber == request.PhoneNumber
                    )
                ), 
                Times.Once);
    }
}
