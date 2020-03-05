using System;

namespace IntegrationEvents.User.Invitation
{
	public class RecruitInviteSendingRequested : IntegrationEvent
	{
		public Guid InviteId { get; set; }
		public Guid InviteeId { get; set; }
		public string Link { get; set; }
	}
}