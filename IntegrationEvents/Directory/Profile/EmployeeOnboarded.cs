using System;

namespace IntegrationEvents.Directory.Profile
{
	public class EmployeeOnboarded : IntegrationEvent
	{
		public EmployeeOnboarded(Guid tenantId, Guid personId) : base(tenantId)
		{
			PersonId = personId;
		}

		public Guid PersonId { get; set; }

		public override string ToString()
		{
			return $"{nameof(EmployeeOnboarded)} : {Id}";
		}
	}

	public class HRManagerOnboarded : IntegrationEvent
	{
		public HRManagerOnboarded(Guid tenantId, Guid personId) : base(tenantId)
		{
			PersonId = personId;
		}

		public Guid PersonId { get; set; }

		public override string ToString()
		{
			return $"{nameof(HRManagerOnboarded)} : {Id}";
		}
	}

	public class DirectorOnboarded : IntegrationEvent
	{
		public DirectorOnboarded(Guid tenantId, Guid personId) : base(tenantId)
		{
			PersonId = personId;
		}

		public Guid PersonId { get; set; }

		public override string ToString()
		{
			return $"{nameof(DirectorOnboarded)} : {Id}";
		}
	}
}