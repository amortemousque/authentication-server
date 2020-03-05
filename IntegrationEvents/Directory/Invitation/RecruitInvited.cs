using System;

namespace IntegrationEvents.Directory.Invitation
{
	public class RecruitInvited : IntegrationEvent
	{
		public RecruitInvited(Guid recruitId, Guid tenantId) : base(tenantId)
		{
			RecruitId = recruitId;
		}

		public Guid RecruitId { get; set; }
		public string Email { get; set; }

		public override string ToString()
		{
			return $"{nameof(RecruitInvited)} : {Id}";
		}
	}
}