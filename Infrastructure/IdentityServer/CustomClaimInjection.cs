using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using AuthorizationServer.Domain;
using AuthorizationServer.Infrastructure.Context;

namespace AuthorizationServer.Infrastructure.IdentityServer
{
	public class CustomClaimInjection : ICustomTokenRequestValidator
	{
		private readonly string _tenantName;
		private readonly Guid _tenantId;

		public CustomClaimInjection(IIdentityService identityService)
		{
			_tenantName = identityService.GetTenantName();
			_tenantId = identityService.GetTenantIdentity();
		}

		public Task ValidateAsync(CustomTokenRequestValidationContext context)
		{
			var claims = context.Result.ValidatedRequest.ClientClaims;
			context.Result.ValidatedRequest.Client.ClientClaimsPrefix = "";
			claims.Add(new Claim(CustomClaimTypes.TenantName, _tenantName));
			claims.Add(new Claim(CustomClaimTypes.TenantId, _tenantId.ToString()));

			return Task.FromResult(0);
		}
	}
}