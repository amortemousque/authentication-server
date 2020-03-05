using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.TenantAggregate;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AuthorizationServer.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ApplicationDbContext _context;

        public TenantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tenant> GetById(Guid id)
        {
            return await _context.Tenants.AsQueryable().SingleOrDefaultAsync(t => t.ArchivedAt == null && t.Id == id);
        }

        public async Task<Tenant> GetByName(string name) 
        {
            var normalizedName = name?.ToLower();
            return await _context.Tenants.AsQueryable().SingleOrDefaultAsync(t => t.ArchivedAt == null && t.Name == normalizedName);
        }

        public async Task<List<Tenant>> List()
        {
            return await _context.Tenants.Find(a => a.ArchivedAt == null).ToListAsync();
        }


        public async Task<IQueryable<Tenant>> GetAll()
        {
            return _context.Tenants.AsQueryable().Where(a => a.ArchivedAt == null);
        }

        public async Task Add(Tenant entity)
        {
            await _context.Tenants.InsertOneAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            await _context.Tenants.DeleteOneAsync(c => c.Id == id);
        }

        public async Task SaveAsync(Tenant entity)
        {
            await _context.Tenants.ReplaceOneAsync(doc => doc.Id == entity.Id, entity, new UpdateOptions() { IsUpsert = true });
        }

        public async Task<bool> HasUniqName(string name)
        {
            return !await _context.Tenants.AsQueryable().AnyAsync(a => a.ArchivedAt == null && a.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> HasUniqArchivedName(string name)
        {
            return !await _context.Tenants.AsQueryable().AnyAsync(a => a.ArchivedAt != null && a.Name.ToLower() == name.ToLower());
        }
    }
}
            