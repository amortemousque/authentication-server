using System;

namespace IntegrationEvents.Directory.Entries
{
	public class EntryCreated : IntegrationEvent
	{
		public Guid PersonId { get; set; }
	}

	public class SupportCollaboratorCreated : EntryCreated{}
	public class CandidateCreated : EntryCreated{}
}