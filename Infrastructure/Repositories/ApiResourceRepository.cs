using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuthorizationServer.Domain.ApiAggregate;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AuthorizationServer.Infrastructure.Repositories
{
    public class ApiResourceRepository : IApiResourceRepository
    {
        private readonly ApplicationDbContext _context;

        public ApiResourceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResource> GetById(Guid id)
        {
            return await _context.ApiResources.AsQueryable<ApiResource>().SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<ApiResource>> List()
        {
            return await _context.ApiResources.Find(_ => true).ToListAsync();
        }

        public async Task<IQueryable<ApiResource>> GetAll()
        {
            return _context.ApiResources.AsQueryable();
        }

        public async Task Add(ApiResource entity)
        {
            await _context.ApiResources.InsertOneAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            await _context.ApiResources.DeleteOneAsync(c => c.Id == id);
        }

        public async Task SaveAsync(ApiResource entity)
        {
            await _context.ApiResources.ReplaceOneAsync(doc => doc.Id == entity.Id, entity, new UpdateOptions() { IsUpsert = true });
        }

        public async Task<bool> HasUniqName(string name) 
        {
            return ! await _context.ApiResources.AsQueryable().AnyAsync(a => a.Name.ToLower() == name.ToLower());
        }

    }
}
    