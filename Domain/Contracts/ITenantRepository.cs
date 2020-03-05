using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.TenantAggregate;

namespace AuthorizationServer.Domain.Contracts
{
    public interface ITenantRepository 
    {
        Task<Tenant> GetById(Guid id);

        Task<Tenant> GetByName(string name);

        Task<List<Tenant>> List();

        Task<IQueryable<Tenant>> GetAll();

        Task Add(Tenant entity);

        Task SaveAsync(Tenant entity);

        Task<bool> HasUniqName(string name);

        Task<bool> HasUniqArchivedName(string name);

    }
}
                