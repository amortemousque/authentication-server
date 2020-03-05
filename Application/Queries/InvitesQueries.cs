using System;
using System.Threading.Tasks;
using AuthorizationServer.Domain.AccountInviteAggregate;
using AuthorizationServer.Infrastructure.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace AuthorizationServer.Application.Queries
{
	public interface IInvitesQueries
	{
		Task<AccountInvite> GetById(Guid id);
	}

	public class InvitesQueries : IInvitesQueries
	{
		private readonly ApplicationDbContext _context;

		public InvitesQueries(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<AccountInvite> GetById(Guid id)
		{
			return await _context.Invites.AsQueryable().FirstOrDefaultAsync(i => i.Id == id);
		}
	}
}