namespace Remp.Common.Helpers.ApiResponse;

public class PutResponse : BaseApiResponse
{
    public string Message { get; set; } = null!;

    public PutResponse(bool success, string message = "Updated successfully") : base(success)
    {
        Message = message;
    }
}
