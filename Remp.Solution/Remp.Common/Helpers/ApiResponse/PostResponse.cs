namespace Remp.Common.Helpers.ApiResponse;

public class PostResponse<T> : BaseApiResponse
{
    public string Message { get; set; } = null!;
    public T? Data { get; set; }

    public PostResponse(bool success, T? data, string message = "Created successfully") : base(success)
    {
        Data = data;
        Message = message;
    }
}
