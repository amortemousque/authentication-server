using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.IdenityServer;

namespace AuthorizationServer.Domain.Contracts
{
    public interface IPersistedGrandRepository 
    {
        Task<PersistedGrant> GetById(Guid id);

        Task<List<PersistedGrant>> List();

        Task Add(PersistedGrant entity);

        Task SaveAsync(PersistedGrant entity);
    }
}
                