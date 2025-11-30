namespace Remp.Service.DTOs;

public class UpdatePasswordResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }

    public UpdatePasswordResponseDto(bool success, string message, string? error = null)
    {
        Success = success;
        Message = message;
        Error = error;
    }
}
