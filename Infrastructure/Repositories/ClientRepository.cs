using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.ClientAggregate;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AuthorizationServer.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Client> GetById(Guid id)
        {
            return await _context.Clients.AsQueryable<Client>().SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Client>> List()
        {
            return await _context.Clients.Find(_ => true).ToListAsync();
        }


        public async Task<IQueryable<Client>> GetAll()
        {
            return _context.Clients.AsQueryable();
        }

        public async Task Add(Client entity)
        {
            await _context.Clients.InsertOneAsync(entity);
        }

        public async Task Delete(Guid id)
        {
            await _context.Clients.DeleteOneAsync(c => c.Id == id);
        }

        public async Task SaveAsync(Client entity)
        {
            await _context.Clients.ReplaceOneAsync(doc => doc.Id == entity.Id, entity, new UpdateOptions() { IsUpsert = true });
        }

        public async Task<bool> HasUniqName(string name, Guid? id = null)
        {
            if (id == null) {
                return !await _context.Clients.AsQueryable().AnyAsync(a => a.ClientName.ToLower() == name.ToLower());
            } else {
                return !await _context.Clients.AsQueryable().AnyAsync(a => a.ClientName.ToLower() == name.ToLower() && a.Id != id);
            }
        }

    }
}
        