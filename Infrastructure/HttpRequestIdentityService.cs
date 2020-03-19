
using System;
using System.Security.Claims;
using AuthorizationServer.Domain;
using IdentityModel;
using Microsoft.AspNetCore.Http;

namespace AuthorizationServer.Infrastructure
{
    internal class HttpRequestIdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public HttpRequestIdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Guid GetUserIdentity()
        {
            var userId = _context.HttpContext.User.FindFirst(JwtClaimTypes.Subject)?.Value;
            return string.IsNullOrWhiteSpace(userId) ? new Guid() : Guid.Parse(userId);
        }

        public Guid GetTenantIdentity()
        {
            return Guid.Parse(_context.HttpContext.User.FindFirst(CustomClaimTypes.TenantId).Value);
        }

        public string GetTenantName()
        {
            return _context.HttpContext.User.FindFirst(CustomClaimTypes.TenantName).Value;
        }
    }
}
