using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers.ApiResponse;
using Remp.Common.Utilities;
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
        private readonly ILoggerService _loggerService;

        public AuthController(IAuthService authService, ILoggerService loggerService)
        {
            _authService = authService;
            _loggerService = loggerService;
        }
        
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="loginRequest">
        /// The payload containing the email and password of the user.
        /// </param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns the jwt token.
        /// </returns>
        /// <response code="200">User logged in</response>
        /// <response code="400">Email or password is invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PostResponse<string>>> Login([FromBody] LoginRequestDto loginRequest, IValidator<LoginRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(loginRequest);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                string errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                // Log
                await _loggerService.LogLogin(
                    email: loginRequest.Email,
                    userId: null,
                    error: errors
                    );

                return ValidationProblem(ModelState);
            }

            var result = await _authService.LoginAsync(loginRequest);
            return Ok(new PostResponse<string>(true, result, "Login successfully"));
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="registerRequest">
        /// The payload containing the details of the user to register.
        /// </param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns the jwt token.
        /// </returns>
        /// <response code="201">User registered.</response>
        /// <response code="400">Request validation failed.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PostResponse<string>>> Register(
            [FromForm] RegisterRequestDto registerRequest,
            [FromServices] IValidator<RegisterRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(registerRequest);
            if (!validationResult.IsValid)
            {
                validationResult.AddToModelState(ModelState);
                string errors = string.Join(" | ", validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

                await _loggerService.LogRegister(
                    email: registerRequest.Email,
                    userId: null,
                    error: errors
                );

                return ValidationProblem(ModelState);
            }

            var result = await _authService.RegisterAsync(registerRequest);
            
            return StatusCode(201, new PostResponse<string>(true, result, "Registered successfully"));
        }
    }
}
