using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Linq;
using AuthorizationServer.Domain;
using IdentityUser = AuthorizationServer.Domain.UserAggregate.IdentityUser;

namespace AuthorizationServer.Infrastructure.IdentityServer
{
    public class CustomProfileService : IProfileService
    {
        protected readonly ILogger Logger;

        protected readonly IUserRepository _userRepository; 

        public UserManager<IdentityUser> _userManager { get; }
        public IRoleRepository _roleRepository { get; }
        public ITenantRepository _tenantRepository { get; }

        public CustomProfileService(UserManager<IdentityUser> userManager, IRoleRepository roleRepository, ITenantRepository tenantRepository, ILogger<CustomProfileService> logger)
        {
            _userManager = userManager;
            _roleRepository = roleRepository;
            _tenantRepository = tenantRepository;
            Logger = logger;
        }


        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null) return;

            var tenant = await _tenantRepository.GetById(user.TenantId);
            var claims = new List<Claim> {
                new Claim(JwtClaimTypes.Subject, subjectId ?? ""),
                new Claim(JwtClaimTypes.Name, user.FullName ?? ""),
                new Claim(JwtClaimTypes.GivenName, user.GivenName ?? ""),
                new Claim(JwtClaimTypes.FamilyName, user.FamilyName ?? ""),
                new Claim(JwtClaimTypes.Email, user.Email ?? ""),
                new Claim(JwtClaimTypes.Picture, user.Picture ?? ""),
                new Claim(JwtClaimTypes.Locale, user.Culture ?? ""),
                new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString(), ClaimValueTypes.Boolean),
                new Claim(CustomClaimTypes.TenantId, user.TenantId.ToString()),
                new Claim(CustomClaimTypes.TenantName, tenant?.Name ?? "")
            };

            claims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
            claims.Add(new Claim(JwtClaimTypes.Subject, subjectId ?? ""));

            var roles = await _userManager.GetRolesAsync(user);
            if (context.RequestedClaimTypes.Contains(JwtClaimTypes.Role))
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                }
            }

            if (context.RequestedClaimTypes.Contains(CustomClaimTypes.Permission)) {
                var permissions = await _roleRepository.GetRolePermissions(roles.ToArray());
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim(CustomClaimTypes.Permission, permission));
                }
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            if (user != null) {
                context.IsActive =  !(await _userManager.IsLockedOutAsync(user)) && await _userManager.IsEmailConfirmedAsync(user);
            } else {
                context.IsActive = false;
            }
        }
    }
}
