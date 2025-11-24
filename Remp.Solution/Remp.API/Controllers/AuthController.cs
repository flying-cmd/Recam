using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Remp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="loginRequest">
        /// The payload containing the email and password of the user.
        /// </param>
        /// <returns>
        /// Returns the jwt token.
        /// </returns>
        /// <response code="200">User logged in</response>
        /// <response code="400">Email or password is invalid</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest, IValidator<LoginRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(loginRequest);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                UserActivityLog.LogLogin(
                    email: loginRequest.Email,
                    userId: null,
                    description: $"User failed to login with erros: {errors}"
                );
                return ValidationProblem(problemDetails);
            }

            var result = await _authService.LoginAsync(loginRequest);
            return Ok(result);
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="registerRequest">
        /// The payload containing the details of the user to register.
        /// </param>
        /// <returns>
        /// Returns the jwt token.
        /// </returns>
        /// <response code="200">User registered.</response>
        /// <response code="400">Request validation failed.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequestDto registerRequest, IValidator<RegisterRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(registerRequest);
            if (!validationResult.IsValid)
            {
                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                UserActivityLog.LogRegister(
                    email: registerRequest.Email,
                    userId: null,
                    description: $"User failed to register with erros: {errors}"
                );
                return ValidationProblem(problemDetails);
            }

            // TODO: Save avatar and return string path

            var registerUser = new RegisterUserDto
            (
                email: registerRequest.Email,
                password: registerRequest.Password,
                confirmPassword: registerRequest.ConfirmPassword,
                firstName: registerRequest.FirstName,
                lastName: registerRequest.LastName,
                companyName: registerRequest.CompanyName,
                avatarUrl: "http://test.com/avatar.jpg" // TODO: hardcode for test now, need to be changed later
            );

            var result = await _authService.RegisterAsync(registerUser);
            return Ok(result);
        }
    }
}
