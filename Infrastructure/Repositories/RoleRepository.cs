using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.PermissionAggregate;
using AuthorizationServer.Domain.RoleAggregate;
using AuthorizationServer.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using IdentityRole = AuthorizationServer.Domain.RoleAggregate.IdentityRole;

namespace AuthorizationServer.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository, IQueryableRoleStore<IdentityRole>
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<IdentityRole>> GetAll()
        {
            return _context.Roles.AsQueryable();
        }


        public async Task<List<string>> GetRolePermissions(string[] roleNames)
        {
            var filterDef = new FilterDefinitionBuilder<IdentityRole>();
            var filter = filterDef.In(x => x.NormalizedName, roleNames);
            var roles = await _context.Roles.Find(filter).ToListAsync() ?? new List<IdentityRole>();
            var permissions = roles.ToList().SelectMany(r => r.Permissions ?? new List<string>()).Distinct().ToList();

            return permissions;
        }

        public async Task<List<Permission>> GetRolePermissions(Guid id)
        {
            var role = await _context.Roles.AsQueryable().SingleOrDefaultAsync(t => t.Id == id.ToString());

            var rolePermissions = role.Permissions ?? new List<string>();


            var filterDef = new FilterDefinitionBuilder<Permission>();
            var filter = filterDef.In(x => x.Name, rolePermissions);
            var permissions = await _context.Permissions.Find(filter).ToListAsync();

            return permissions;
        }

        public async Task<bool> HasUniqName(string name)
        {
            return !await _context.Roles.AsQueryable().AnyAsync(a => a.Name.ToLower() == name.ToLower());
        }



        public virtual void Dispose()
        {
            // no need to dispose of anything, mongodb handles connection pooling automatically
        }

        public virtual async Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken token)
        {
            await _context.Roles.InsertOneAsync(role, cancellationToken: token);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken token)
        {
            var result = await _context.Roles.ReplaceOneAsync(r => r.Id == role.Id, role, cancellationToken: token);
            // todo low priority result based on replace result
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken token)
        {
            var result = await _context.Roles.DeleteOneAsync(r => r.Id == role.Id, token);
            // todo low priority result based on delete result
            return IdentityResult.Success;
        }

        public virtual async Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
            => role.Id;

        public virtual async Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => role.Name;

        public virtual async Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
            => role.Name = roleName;

        // note: can't test as of yet through integration testing because the Identity framework doesn't use this method internally anywhere
        public virtual async Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
            => role.NormalizedName;

        public virtual async Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
            => role.NormalizedName = normalizedName;

        public virtual Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken token)
            => _context.Roles.Find(r => r.Id == roleId)
                .FirstOrDefaultAsync(token);

        public virtual Task<IdentityRole> FindByNameAsync(string normalizedName, CancellationToken token)
            => _context.Roles.Find(r => r.NormalizedName == normalizedName)
                .FirstOrDefaultAsync(token);

       

        public virtual IQueryable<IdentityRole> Roles
            => _context.Roles.AsQueryable();


    }
}
    