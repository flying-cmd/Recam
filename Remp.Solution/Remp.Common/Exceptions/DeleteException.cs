using System.Net;

namespace Remp.Common.Exceptions;

public class DeleteException : BaseException
{
    public DeleteException(string message, string title, HttpStatusCode statusCode = HttpStatusCode.BadRequest) : base(message, title, statusCode) 
    {
    }
}
