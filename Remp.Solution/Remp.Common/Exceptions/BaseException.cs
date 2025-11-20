using System.Net;

namespace Remp.Common.Exceptions;

public class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public string Title { get; set; }
    public BaseException(string message, string title, HttpStatusCode statusCode) : base(message)
    {
        Title = title;
        StatusCode = statusCode;
    }
}
