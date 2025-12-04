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
        var validatorMock = new Mock<IValidator<LoginRequestDto>>();
        validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _authServiceMock.Setup(s => s.LoginAsync(request)).ReturnsAsync("jwt-token");

        // Act
        var result = await _authController.Login(request, validatorMock.Object);

        // Assert
        var okResult = result.Result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);

        var response = okResult.Value as PostResponse<string>;
        response!.Success.Should().BeTrue();
        response.Data.Should().Be("jwt-token");
        response.Message.Should().Be("Login successfully");
        validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(a => a.LoginAsync(request), Times.Once);
    }

    [Fact]
    public async Task Login_WhenRequestIsInvalid_ShouldReturnValidationProblem()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            Email = "invalid-email",
            Password = "password"
        };
        var validatorMock = new Mock<IValidator<LoginRequestDto>>();
        var validationFailure = new ValidationFailure("Email", "Invalid email address");
        validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new [] { validationFailure }));
    
        // Act
        var result = await _authController.Login(request, validatorMock.Object);
    
        // Assert
        var badRequestResult = result.Result as ObjectResult;
        badRequestResult.Should().NotBeNull();

        var problem = badRequestResult.Value as ValidationProblemDetails;
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey("Email");

        validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock.Verify(a => a.LoginAsync(request), Times.Never);
    }

    [Fact]
    public async Task Register_WhenRequestIsValid_ShouldReturnOk()
    {
        // Arrange
        var avatarFile = new Mock<IFormFile>();
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "password",
            ConfirmPassword = "password",
            FirstName = "John",
            LastName = "Doe",
            Avatar = avatarFile.Object
        };

        var validatorMock = new Mock<IValidator<RegisterRequestDto>>();
        validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _authServiceMock
            .Setup(s => s.RegisterAsync(
                It.Is<RegisterRequestDto>(
                    u => u.Email == request.Email &&
                    u.Password == request.Password &&
                    u.ConfirmPassword == request.ConfirmPassword &&
                    u.FirstName == request.FirstName &&
                    u.LastName == request.LastName
                    )
                )
            )
            .ReturnsAsync("jwt-token");
    
        // Act
        var result = await _authController.Register(request, validatorMock.Object);
    
        // Assert
        var okResult = result.Result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(201);
    
        var response = okResult.Value as PostResponse<string>;
        response!.Success.Should().BeTrue();
        response.Data.Should().Be("jwt-token");
        response.Message.Should().Be("Registered successfully");

        validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock
            .Verify(a => a.RegisterAsync(
                It.Is<RegisterRequestDto>(
                    u => u.Email == request.Email &&
                    u.Password == request.Password &&
                    u.ConfirmPassword == request.ConfirmPassword &&
                    u.FirstName == request.FirstName &&
                    u.LastName == request.LastName
                    )
                ), 
                Times.Once);
    }

    [Fact]
    public async Task Register_WhenRequestIsInvalid_ShouldReturnValidationProblem()
    {
        // Arrange
        var avatarFile = new Mock<IFormFile>();
        var request = new RegisterRequestDto
        {
            Email = "invalid-email",
            Password = "password",
            ConfirmPassword = "password",
            FirstName = "",
            LastName = "",
            Avatar = avatarFile.Object
        };

        var validatorMock = new Mock<IValidator<RegisterRequestDto>>();
        var emailFailure = new ValidationFailure("Email", "Invalid email address");
        var firtNameFailure = new ValidationFailure("FirstName", "First name is required");
        var lastNameFailure = new ValidationFailure("LastName", "Last name is required");

        validatorMock
            .Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new [] { emailFailure, firtNameFailure, lastNameFailure }));

        // Act
        var result = await _authController.Register(request, validatorMock.Object);
    
        // Assert
        var badRequestResult = result.Result as ObjectResult;
        badRequestResult.Should().NotBeNull();

        var problem = badRequestResult.Value as ValidationProblemDetails;
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey("Email");
        problem.Errors.Should().ContainKey("FirstName");
        problem.Errors.Should().ContainKey("LastName");

        validatorMock.Verify(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _authServiceMock
            .Verify(a => a.RegisterAsync(
                It.IsAny<RegisterRequestDto>()), 
                Times.Never);
    }
}
