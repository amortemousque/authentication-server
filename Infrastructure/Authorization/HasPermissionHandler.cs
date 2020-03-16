using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using AuthorizationServer.Exceptions;
using Microsoft.AspNetCore.Http;
using AuthorizationServer.Infrastructure.IdentityServer.Extensions;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using AuthorizationServer.Domain;
using System.Collections.Generic;
using AuthorizationServer.Domain.RoleAggregate;

namespace AuthorizationServer
{
    public class HasPermissionHandler : AuthorizationHandler<HasPermissionRequirement>
    {
	    private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public HasPermissionHandler(IHttpContextAccessor contextAccessor, ApplicationDbContext context)
	    {
		    _contextAccessor = contextAccessor;
            _dbContext = context;

        }

        private List<string> GetRolePermissions(string[] roleNames)
        {
            var filterDef = new FilterDefinitionBuilder<IdentityRole>();
            var filter = filterDef.In(x => x.NormalizedName, roleNames);
            var roles = _dbContext.Roles.Find(filter).ToList() ?? new List<IdentityRole>();
            var permissions = roles.ToList().SelectMany(r => r.Permissions ?? new List<string>()).Distinct().ToList();

            return permissions;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
        {
            var issuer = requirement.Issuer;
            var uri = new Uri(issuer);
            var issuerHost = uri.Host;

            var requestedTenantName = _contextAccessor.HttpContext.Request.GetTenantNameFromHost();
            var tokenTenantName = context.User.FindFirst(CustomClaimTypes.TenantName)?.Value.ToLower();


            if (tokenTenantName != requestedTenantName)
	            throw new ForbbidenException($"request url host '{requestedTenantName}' is different to the tokenized tenant '{tokenTenantName}'");

            var tokenTenantId = context.User.FindFirst(CustomClaimTypes.TenantId) != null ? Guid.Parse(context.User.FindFirst(CustomClaimTypes.TenantId)?.Value) : Guid.Empty;
            var user = _dbContext.Users.AsQueryable().First(u => u.NormalizedUserName == context.User.Identity.Name.ToUpper() && u.TenantId == tokenTenantId);
            var permissions = GetRolePermissions(user.Roles.ToArray());

            if (permissions.Any(p => p.Contains(requirement.Permission)))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
