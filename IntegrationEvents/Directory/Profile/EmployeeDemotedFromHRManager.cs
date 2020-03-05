using System;

namespace IntegrationEvents.Directory.Profile
{
	public class EmployeeDemotedFromHRManager : IntegrationEvent
	{
		public EmployeeDemotedFromHRManager(Guid tenantId, Guid employeeId) : base(tenantId)
		{
			EmployeeId = employeeId;
		}

		public Guid EmployeeId { get; set; }

		public override string ToString()
		{
			return $"{nameof(EmployeeDemotedFromHRManager)} : {Id}";
		}
	}
}