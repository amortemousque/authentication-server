using System;
using System.Threading.Tasks;
using AuthorizationServer.Domain.AccountInviteAggregate;

namespace AuthorizationServer.Domain.Contracts
{
	public interface IAccountInviteRepository
	{
		Task<AccountInvite> GetById(Guid inviteId, Guid tenantId);
		Task SaveAsync(AccountInvite entity);
		Task<AccountInvite> GetOpenInviteForPerson(Guid inviteeId, Guid tenantId, string role);
	}
}