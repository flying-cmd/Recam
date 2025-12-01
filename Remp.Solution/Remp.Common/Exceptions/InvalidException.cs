using System.Net;

namespace Remp.Common.Exceptions;

public class InvalidException : BaseException
{
    public InvalidException(string message, string title, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, title, statusCode) { }
}
