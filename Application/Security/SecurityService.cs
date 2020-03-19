using System;
using System.Threading.Tasks;
using AuthorizationServer.Infrastructure;
using AuthorizationServer.Infrastructure.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AuthorizationServer.Application.Security
{
    public class SecurityService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IIdentityService _identityService;

        public SecurityService(IAuthorizationService authorizationService, IHttpContextAccessor contextAccessor, IIdentityService identityService)
        {
            _authorizationService = authorizationService;
            _contextAccessor = contextAccessor;
            _identityService = identityService;
        }

        public async Task<bool> CanReadRoleRoot()
        {

            var authorizationResult = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, "roles:read:root");
            return authorizationResult.Succeeded;
        }

        public async Task<bool> CanReadUser(Guid userId, Guid userTenantId)
        {
            var isRoot = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, "users:read:root");
            var isClient = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, $"application(users:read)");
            var isAdmin = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, "users:read:tenant");
            var isUser = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, $"users:read:user");
            return (isAdmin.Succeeded && _identityService.GetTenantIdentity() == userTenantId) || 
                   (isUser.Succeeded && _identityService.GetUserIdentity() == userId) || isClient.Succeeded || isRoot.Succeeded;
        }

        public async Task<bool> CanCreateUser()
        {
            var isRoot = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, "users:write:root");
            var isClient = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, $"application(users:write)");
            var isAdmin = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, "users:write:tenant");
            return isAdmin.Succeeded || isClient.Succeeded || isRoot.Succeeded;
        }

        public async Task<bool> CanUpdateUser(Guid userId, Guid userTenantId)
        {
            var isRoot = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, "users:write:root");
            var isAdmin = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, $"users:write:tenant");
            var isClient = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, $"application(users:write)");
            var isUser = await _authorizationService.AuthorizeAsync(_contextAccessor.HttpContext.User, $"users:write:user");
            return (isAdmin.Succeeded && _identityService.GetTenantIdentity() == userTenantId) || 
                   (isUser.Succeeded && _identityService.GetUserIdentity() == userId) || isClient.Succeeded || isRoot.Succeeded;
        }
    }
}
