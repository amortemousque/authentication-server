using System;
namespace AuthorizationServer.Exceptions
{
    public class ForbbidenException : Exception
    {
        public ForbbidenException() : base("Invalid credentials")
        { }

        public ForbbidenException(string message)
            : base(message)
        { }

        public ForbbidenException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
