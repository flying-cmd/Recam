namespace Remp.Common.Helpers.ApiResponse;

public class GetResponse<T> : BaseApiResponse
{
    public T? Data { get; set; }

    public GetResponse(bool success, T? data) : base(success)
    {
        if (data != null)
        {
            Data = data;
        }
        //Data = nul
    }
}
