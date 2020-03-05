using System;
using IdentityModel;
using IdentityServer4.Models;

namespace AuthorizationServer.Infrastructure.IdentityServer
{
    public static class CustomIdentityResources
    {
        
        public class Tenant : IdentityResource
        {
            public Tenant(): base("tenant", new[] { AuthorizationServer.Domain.CustomClaimTypes.TenantId, AuthorizationServer.Domain.CustomClaimTypes.TenantName }) 
            {
                this.DisplayName = "Tenant informations";
            }
        }

        public class Role : IdentityResource
        {
            public Role() : base("role", new[] { JwtClaimTypes.Role })
            {
                this.DisplayName = "User role list";
            }
        }

        public class Scope : IdentityResource
        {
            public Scope() : base("scope", new[] { JwtClaimTypes.Scope })
            {
                this.DisplayName = "User api scope list";
            }
        }

        public class Permission : IdentityResource
        {
            public Permission(): base("permission", new[] { AuthorizationServer.Domain.CustomClaimTypes.Permission }) 
            {
                this.DisplayName = "User permissions";
            }
        }

    }
}
