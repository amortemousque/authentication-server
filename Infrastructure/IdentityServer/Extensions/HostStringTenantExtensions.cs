using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AuthorizationServer.Infrastructure.IdentityServer.Extensions
{
    public static class HostStringTenantExtensions
    {
 
        public static string GetTenantNameFromHost(this HttpRequest request)
        {
            var tenantName = request.Host.Host.Split('.').First();
            if(tenantName == "localhost") tenantName = "sandbox";
            return tenantName;
        }
    }
}
