using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
using System.Linq.Expressions;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.PermissionAggregate;
using AuthorizationServer.Domain.RoleAggregate;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AuthorizationServer.Application.Security;

namespace AuthorizationServer.Application.Queries
{
    public class RoleQueries
    {
        private readonly IRoleRepository _roleRepository;
        private readonly SecurityService _securityService;
        public RoleQueries(IRoleRepository roleRepository, SecurityService securityService)
        {
            _roleRepository = roleRepository;
            _securityService = securityService;
        }

        public async Task<List<IdentityRole>> GetRolesAsync(string name)
        {
            var response = await _roleRepository.GetAll();

            if (! await _securityService.CanReadRoleRoot()) 
            {
                response = response.Where(a => a.NormalizedName != "ROOT");
            }

            if (!string.IsNullOrWhiteSpace(name))
            {
                var nameUpper = name.ToUpper();
                response = response.Where(a => a.Name.ToLower().Equals(name));
            }

            return response.ToList();
        }
    }
}
