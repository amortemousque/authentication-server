using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.IdenityServer;
using AuthorizationServer.Domain.RessourceAggregate;

namespace AuthorizationServer.Domain.Contracts
{
    public interface IIdentityResourceRepository 
    {

        Task<IdentityResource> GetById(Guid id);

        Task<List<IdentityResource>> List();

        Task Add(IdentityResource entity);

        Task SaveAsync(IdentityResource entity);
    }
}
                    