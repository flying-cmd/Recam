namespace Remp.Common.Helpers;

public class UpdateApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Error { get; set; }

    public UpdateApiResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public UpdateApiResponse(bool success, string message, string error)
    {
        Success = success;
        Message = message;
        Error = error;
    }
}
