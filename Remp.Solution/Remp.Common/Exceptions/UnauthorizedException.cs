using System.Net;

namespace Remp.Common.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message, string title) : base(message, title, HttpStatusCode.Unauthorized)
    {
    }
}
