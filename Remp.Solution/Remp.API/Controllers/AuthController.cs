using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Remp.Common.Helpers.ApiResponse;
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
            return Ok(new PostResponse<string>(true, result, "Login successfully"));
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="registerRequest">
        /// The payload containing the details of the user to register.
        /// </param>
        /// <param name="blobStorageService"></param>
        /// <param name="validator"></param>
        /// <returns>
        /// Returns the jwt token.
        /// </returns>
        /// <response code="200">User registered.</response>
        /// <response code="400">Request validation failed.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostResponse<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<ActionResult<PostResponse<string>>> Register(
            [FromForm] RegisterRequestDto registerRequest, 
            [FromServices] IBlobStorageService blobStorageService,
            [FromServices] IValidator<RegisterRequestDto> validator)
        {
            // Validate
            var validationResult = await validator.ValidateAsync(registerRequest);
            if (!validationResult.IsValid)
            {
                foreach(var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                var problemDetails = new ValidationProblemDetails(validationResult.ToDictionary());
                string errors = string.Join("| ", problemDetails.Errors.Select(e => $"{e.Key}: {string.Join(" ", e.Value)}"));

                UserActivityLog.LogRegister(
                    email: registerRequest.Email,
                    userId: null,
                    description: $"User failed to register with erros: {errors}"
                );
                return ValidationProblem(ModelState);
            }

            // Upload avatar to Azure blob storage
            var avatarUrl = await blobStorageService.UploadFileAsync(registerRequest.Avatar);

            var registerUser = new RegisterUserDto
            (
                email: registerRequest.Email,
                password: registerRequest.Password,
                confirmPassword: registerRequest.ConfirmPassword,
                firstName: registerRequest.FirstName,
                lastName: registerRequest.LastName,
                companyName: registerRequest.CompanyName,
                avatarUrl: avatarUrl
            );

            var result = await _authService.RegisterAsync(registerUser);
            
            return Ok(new PostResponse<string>(true, result, "Registered successfully"));
        }
    }
}
