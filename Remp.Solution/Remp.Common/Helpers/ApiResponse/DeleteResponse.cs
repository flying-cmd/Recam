namespace Remp.Common.Helpers.ApiResponse;

public class DeleteResponse : BaseApiResponse
{
    public string Message { get; set; } = null!;
    public DeleteResponse(bool success, string message = "Deleted successfully") : base(success)
    {
        Message = message;
    }
}
