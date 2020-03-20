
using System;
using System.Security.Claims;
using AuthorizationServer.Application.Queries;
using AuthorizationServer.Domain;
using AuthorizationServer.Infrastructure.IdentityServer.Extensions;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace AuthorizationServer.Infrastructure
{
    internal class HttpRequestIdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        private readonly TenantQueries _tenantQuery;

        public HttpRequestIdentityService(IHttpContextAccessor context, TenantQueries tenantQuery)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _tenantQuery = tenantQuery;
        }

        public Guid GetUserIdentity()
        {
            var userId = _context.HttpContext.User.FindFirst(JwtClaimTypes.Subject)?.Value;
            return string.IsNullOrWhiteSpace(userId) ? new Guid() : Guid.Parse(userId);
        }

        public Guid GetTenantIdentity()
        {
            var tenant = _tenantQuery.GetTenantByNameAsync(this.GetTenantName()).Result;
            return tenant.Id;
        }

        public string GetTenantName()
        {
            return _context.HttpContext.Request.GetTenantNameFromHost();
        }
    }
}
