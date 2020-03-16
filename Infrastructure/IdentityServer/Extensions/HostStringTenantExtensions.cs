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

        public static Guid? GetTenantIdFromBody(this HttpRequest request)
        {
            Guid? tenantId = null;
            HttpRequestRewindExtensions.EnableBuffering(request);
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 128, true))
            {
                var body = reader.ReadToEnd();
                try
                {
                    var jobject = JObject.Parse(body);
                    var result = jobject.GetValue("tenantId").ToString();
                    tenantId = Guid.Parse(result);
                }
                catch
                {
                }
            }
            //_accessor.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            request.Body.Position = 0;
            return tenantId;
        }
    }
}
