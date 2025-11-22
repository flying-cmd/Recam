using System.Net;

namespace Remp.Common.Exceptions;

public class RegisterException : BaseException
{
    public RegisterException(string message, string title) : base(message, title, HttpStatusCode.BadRequest)
    {
    }
}
