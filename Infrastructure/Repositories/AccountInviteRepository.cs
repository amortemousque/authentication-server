using System;
using System.Threading.Tasks;
using AuthorizationServer.Domain.AccountInviteAggregate;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AuthorizationServer.Infrastructure.Repositories
{
	public class AccountInviteRepository : IAccountInviteRepository
	{
		private readonly ApplicationDbContext _context;

		public AccountInviteRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<AccountInvite> GetById(Guid inviteId, Guid tenantId)
		{
			return await _context.Invites.AsQueryable().FirstOrDefaultAsync(i => i.TenantId == tenantId && i.Id == inviteId);
		}

		public async Task SaveAsync(AccountInvite entity)
		{
			entity.CreatedAt = entity.CreatedAt == DateTime.MinValue ? DateTime.Now : entity.CreatedAt;
			entity.UpdatedAt = DateTime.Now;
			await _context.Invites.InsertOneAsync(entity);
		}

		public async Task<AccountInvite> GetOpenInviteForPerson(Guid inviteeId, Guid tenantId, string role)
		{
			return await _context.Invites.AsQueryable().FirstOrDefaultAsync(i => i.TenantId == tenantId && i.InviteeId == inviteeId 
				&& i.IsAccepted == false && i.Role == role);
		}
	}
}