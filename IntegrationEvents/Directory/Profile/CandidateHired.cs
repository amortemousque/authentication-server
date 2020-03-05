using System;

namespace IntegrationEvents.Directory.Profile
{
	public class CandidateHired : IntegrationEvent
	{
		public CandidateHired(Guid tenantId, Guid candidateId) : base(tenantId)
		{
			CandidateId = candidateId;
		}

		public Guid CandidateId { get; set; }

		public override string ToString()
		{
			return $"{nameof(CandidateHired)} : {Id}";
		}
	}
}