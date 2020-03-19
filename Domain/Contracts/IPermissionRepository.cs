using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.PermissionAggregate;

namespace AuthorizationServer.Domain.Contracts
{
    public interface IPermissionRepository 
    {
        Task<Permission> GetById(Guid id);

        Task<List<Permission>> GetByNames(string[] ids);

        Task<List<Permission>> List();

        Task<IQueryable<Permission>> GetAll();

        Task<List<string>> GetRolePermissions(string[] roleNames);

        Task<List<Permission>> GetRolePermissions(Guid id);

        Task Add(Permission entity);

        Task Delete(Guid id);

        Task SaveAsync(Permission entity);

        Task<bool> HasUniqName(string name);
    }
}
                    