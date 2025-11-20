using System.Net;

namespace Remp.Common.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message, string title) : base(message, title, HttpStatusCode.NotFound) { }
}
