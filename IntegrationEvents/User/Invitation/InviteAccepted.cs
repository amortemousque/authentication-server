using System;

namespace IntegrationEvents.User.Invitation
{
	public class InviteAccepted : IntegrationEvent
	{
		public Guid InviteeId { get; set; }

		public override string ToString()
		{
			return $"{nameof(InviteAccepted)} : {Id}";
		}
	}
}