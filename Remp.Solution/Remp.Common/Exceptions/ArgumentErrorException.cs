using System.Net;

namespace Remp.Common.Exceptions;

public class ArgumentErrorException : BaseException
{
    public ArgumentErrorException(string message, string title, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, title, statusCode)
    {
    }
}
