using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using AuthorizationServer.Exceptions;
using Microsoft.AspNetCore.Http;

namespace AuthorizationServer
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
	    private readonly IHttpContextAccessor _contextAccessor;

	    public HasPermissionHandler(IHttpContextAccessor contextAccessor)
	    {
		    _contextAccessor = contextAccessor;
	    }
		
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            var issuer = requirement.Issuer;
            var uri = new Uri(issuer);
            var issuerHost = uri.Host;

            var requestedTenantName = _contextAccessor.HttpContext.Request.Host.Host.Split('.').First().ToLower();
            var tokenTenantName = context.User.FindFirst("tenant_name")?.Value.ToLower();
            if (tokenTenantName != requestedTenantName)
	            throw new ForbbidenException($"request url host '{requestedTenantName}' is different to the tokenized tenant '{tokenTenantName}'");
			
            if (context.User.HasClaim(c => c.Type == "permission" && c.Value.Contains(requirement.Permission) && c.Issuer.Contains(issuerHost)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
