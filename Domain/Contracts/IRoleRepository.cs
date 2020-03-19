using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.PermissionAggregate;
using AuthorizationServer.Domain.RoleAggregate;

namespace AuthorizationServer.Domain.Contracts
{
    public interface IRoleRepository
    {
        Task<IQueryable<IdentityRole>> GetAll();

        Task<bool> HasUniqName(string name);
    }
}
    