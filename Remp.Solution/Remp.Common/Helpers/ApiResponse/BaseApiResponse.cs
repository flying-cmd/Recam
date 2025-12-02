namespace Remp.Common.Helpers.ApiResponse;

public class BaseApiResponse
{
    public bool Success { get; set; }

    public BaseApiResponse(bool success)
    {
        Success = success;
    }
}
