using System.Net;

namespace Remp.Common.Exceptions;

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message, string title) : base(message, title, HttpStatusCode.Forbidden) { }
}
