using System;
namespace AuthorizationServer.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(Guid resourceId) : base($"Resource not found {resourceId}")
        {

        }

        public NotFoundException(string resourceId) : base($"Resource not found {resourceId}")
        {

        }
    }
}
