namespace Remp.Common.Helpers.ApiResponse;

public class PutResponse : BaseApiResponse
{
    string Message { get; set; } = null!;

    public PutResponse(bool success, string message = "Updated successfully") : base(success)
    {
        Message = message;
    }
}
