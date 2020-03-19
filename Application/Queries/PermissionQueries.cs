using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
using System.Linq.Expressions;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.PermissionAggregate;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using AuthorizationServer.Domain.RoleAggregate;

namespace AuthorizationServer.Application.Queries
{
    public class PermissionQueries
    {
        private readonly ApplicationDbContext _dbContext;

        public PermissionQueries(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<string>> GetUserPermissions(Guid userId)
        {
            var user = await _dbContext.Users.Find(u => u.Id == userId.ToString()).FirstAsync();
            return await GetRolePermissionsAsync(user.Roles);
        }

        public async Task<List<string>> GetRolePermissionsAsync(List<string> roleNames)
        {
            var filterDef = new FilterDefinitionBuilder<IdentityRole>();
            var filter = filterDef.In(x => x.NormalizedName, roleNames);
            var roles = await _dbContext.Roles.Find(filter).ToListAsync();
            if (roles == null)
                roles = new List<IdentityRole>();

            var permissions = roles.ToList().SelectMany(r => r.Permissions ?? new List<string>()).Distinct().ToList();

            return permissions;
        }

        public async Task<List<Permission>> GetRolePermissionsAsync(Guid id)
        {
            var role = await _dbContext.Roles.AsQueryable().SingleOrDefaultAsync(t => t.Id == id.ToString());

            var rolePermissions = role.Permissions ?? new List<string>();


            var filterDef = new FilterDefinitionBuilder<Permission>();
            var filter = filterDef.In(x => x.Name, rolePermissions);
            var permissions = await _dbContext.Permissions.Find(filter).ToListAsync();

            return permissions;
        }

        public async Task<Permission> GetPermissionAsync(Guid id)
        {
            var response = await _dbContext.Permissions.AsQueryable().SingleOrDefaultAsync(t => t.Id == id);

            if (response == null)
                throw new KeyNotFoundException();

            return response;
        }

        public async Task<List<Permission>> GetPermissionsAsync(string name)
        {

            var response = _dbContext.Permissions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                response = response.Where(a => a.Name.StartsWith(name));
            }


            return await response.ToListAsync();
        }

    }

}
