using System;
using AuthorizationServer.Domain.Shared;

namespace AuthorizationServer.Infrastructure.Context
{
    public interface IApplicationContext
    {
        string ClientId { get; }
        PartialUser User { get; set; }
        PartialTenant Tenant { get; set; }
    }
}
