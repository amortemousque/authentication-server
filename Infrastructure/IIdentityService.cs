using System;

namespace AuthorizationServer.Infrastructure
{
    public interface IIdentityService
    {
        Guid GetUserIdentity();

        Guid GetTenantIdentity();

        string GetTenantName();
    }
}
