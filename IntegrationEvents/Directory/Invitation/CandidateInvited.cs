using System;

namespace IntegrationEvents.Directory.Invitation
{
	public class CandidateInvited : IntegrationEvent
	{
		public CandidateInvited(Guid tenantId, Guid candidateId) : base(tenantId)
		{
			CandidateId = candidateId;
		}

		public Guid CandidateId { get; set; }
		public string Email { get; set; }

		public override string ToString()
		{
			return $"{nameof(CandidateInvited)} : {Id}";
		}
	}
}