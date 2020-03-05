using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntegrationEvents.Directory.Invitation;
using IntegrationEvents.User.Invitation;
using AuthorizationServer.Domain.AccountInviteAggregate;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.RoleAggregate;
using Microsoft.Extensions.Configuration;
using Rebus.Bus;
using Rebus.Handlers;

namespace AuthorizationServer.Application.EventHandlers
{
	public class InviteHandlers : IHandleMessages<CandidateInvited>, IHandleMessages<RecruitInvited>
	{
		private readonly IAccountInviteRepository _inviteRepository;
		private readonly ITenantRepository _tenantRepository;
		private readonly IBus _bus;
		private readonly IConfiguration _config;

		public InviteHandlers(IAccountInviteRepository inviteRepository, ITenantRepository tenantRepository, IBus bus, IConfiguration config)
		{
			_inviteRepository = inviteRepository;
			_tenantRepository = tenantRepository;
			_bus = bus;
			_config = config;
		}

		public async Task Handle(CandidateInvited message)
		{
			var invite = await _inviteRepository.GetOpenInviteForPerson(message.CandidateId, message.TenantId, IdentityRole.NormalizedNames.Candidate);
			if (invite != null) return;
			invite = AccountInvite.Factory.CreateNewEntry(message.CandidateId, message.TenantId, message.Email, message.InitiatorId, IdentityRole.NormalizedNames.Candidate);
			await _inviteRepository.SaveAsync(invite);
			var inviteAcceptationUrl = await GenerateInviteAcceptationUrl(invite);
			var ev = new CandidateInviteSendingRequested{InviteId = invite.Id, TenantId = invite.TenantId, InviteeId = invite.InviteeId, Link = inviteAcceptationUrl, InitiatorId = message.InitiatorId, TenantName = message.TenantName};
			await _bus.Publish(ev);
		}

		public async Task Handle(RecruitInvited message)
		{
			var invite = await _inviteRepository.GetOpenInviteForPerson(message.RecruitId, message.TenantId, IdentityRole.NormalizedNames.Recruit);
			if (invite != null) return;
			invite = AccountInvite.Factory.CreateNewEntry(message.RecruitId, message.TenantId, message.Email, message.InitiatorId, IdentityRole.NormalizedNames.Recruit);
			await _inviteRepository.SaveAsync(invite);
			var inviteAcceptationUrl = await GenerateInviteAcceptationUrl(invite);
			var ev = new RecruitInviteSendingRequested { InviteId = invite.Id, TenantId = invite.TenantId, InviteeId = invite.InviteeId, Link = inviteAcceptationUrl, InitiatorId = message.InitiatorId, TenantName = message.TenantName };
			await _bus.Publish(ev);
		}

		private async Task<string> GenerateInviteAcceptationUrl(AccountInvite invite)
		{
			var host = _config["HOST"];
			var tenantName = (await _tenantRepository.GetById(invite.TenantId)).Name;
			//TODO: the invite page doesn't exist yet.
			var inviteAcceptationUrl = $"https://{tenantName}.{host}/account/invite/{invite.Id}";
			return ReplaceTenantDomain(inviteAcceptationUrl, tenantName);
		}

		private string ReplaceTenantDomain(string url, string tenantName)
		{
			return Regex.Replace(url, @"\:\/\/[a-z0-9]+", "://" + tenantName);
		}
	}
}