using System;

namespace IntegrationEvents.Directory.Profile
{
	public class EmployeePromotedToHRManager : IntegrationEvent
	{
		public EmployeePromotedToHRManager(Guid tenantId, Guid employeeId) : base(tenantId)
		{
			EmployeeId = employeeId;
		}

		public Guid EmployeeId { get; set; }

		public override string ToString()
		{
			return $"{nameof(EmployeePromotedToHRManager)} : {Id}";
		}
	}
}