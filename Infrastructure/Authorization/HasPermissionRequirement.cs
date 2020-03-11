using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace AuthorizationServer
{

    public class HasPermissionRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Permission { get; }

        public HasPermissionRequirement(string permission, string issuer)
        {
            Permission = permission ?? throw new ArgumentNullException(nameof(permission));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }
    }
}
