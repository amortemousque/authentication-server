using System;

namespace IntegrationEvents.Directory.Profile
{
	public class EmployeePromotedToManager : IntegrationEvent
	{
		public EmployeePromotedToManager(Guid tenantId, Guid employeeId) : base(tenantId)
		{
			EmployeeId = employeeId;
		}

		public Guid EmployeeId { get; set; }

		public override string ToString()
		{
			return $"{nameof(EmployeePromotedToManager)} : {Id}";
		}
	}
}