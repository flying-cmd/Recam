using System.Net;

namespace Remp.Common.Exceptions;

public class DbErrorException : BaseException
{
    public DbErrorException(string message, string title) : base(message, title, HttpStatusCode.InternalServerError) 
    {
    }
}
