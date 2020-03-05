using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuthorizationServer.Domain.ApiAggregate;

namespace AuthorizationServer.Domain.Contracts
{
    public interface IApiResourceRepository 
    {
        Task<ApiResource> GetById(Guid id);

        Task<List<ApiResource>> List();

        Task<IQueryable<ApiResource>> GetAll();

        Task Delete(Guid id);

        Task Add(ApiResource entity);

        Task SaveAsync(ApiResource entity);

        Task<bool> HasUniqName(string name);
    }
}
        