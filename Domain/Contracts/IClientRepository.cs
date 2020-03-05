using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.ClientAggregate;
using AuthorizationServer.Domain.IdenityServer;

namespace AuthorizationServer.Domain.Contracts
{
    public interface IClientRepository 
    {
        Task<Client> GetById(Guid id);

        Task<List<Client>> List();

        Task<IQueryable<Client>> GetAll();

        Task Add(Client entity);

        Task Delete(Guid id);

        Task SaveAsync(Client entity);

        Task<bool> HasUniqName(string name, Guid? id = null);
    }
}
            