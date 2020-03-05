using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.IdenityServer;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AuthorizationServer.Infrastructure.Repositories
{
    public class PersistedGrantRepository : IPersistedGrandRepository
    {
        private readonly ApplicationDbContext _context;

        public PersistedGrantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PersistedGrant> GetById(Guid id)
        {
            return await _context.PersistedGrants.AsQueryable<PersistedGrant>().SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<PersistedGrant>> List()
        {
            return await _context.PersistedGrants.Find(_ => true).ToListAsync();
        }

        public async Task Add(PersistedGrant entity)
        {
            await _context.PersistedGrants.InsertOneAsync(entity);
        }

        public async Task SaveAsync(PersistedGrant entity)
        {
            await _context.PersistedGrants.ReplaceOneAsync(doc => doc.Id == entity.Id, entity, new UpdateOptions() { IsUpsert = true });
        }
    }
}
                