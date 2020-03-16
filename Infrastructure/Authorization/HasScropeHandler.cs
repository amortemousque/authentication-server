using System;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using AuthorizationServer.Infrastructure.IdentityServer.Extensions;

namespace AuthorizationServer
{
	public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
	{
		private readonly IHttpContextAccessor _contextAccessor;

		public HasScopeHandler(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
		{
			var issuer = requirement.Issuer;
			var uri = new Uri(issuer);
			var issuerHost = uri.Host;

			var requestedTenantName = _contextAccessor.HttpContext.Request.GetTenantNameFromHost();
			var tokenTenantName = context.User.FindFirst("tenant_name")?.Value.ToLower();
			if (tokenTenantName != requestedTenantName)
				throw new ForbbidenException($"request url host '{requestedTenantName}' is different to the tokenized tenant '{tokenTenantName}'");
			// Succeed if the scope array contains the required scope
			var c = context.User.Claims.ToList();
			if (context.User.HasClaim(c => c.Type == "scope" && c.Value == requirement.Scope && c.Issuer.Contains(issuerHost)))
				context.Succeed(requirement);

			return Task.CompletedTask;
		}
	}
}