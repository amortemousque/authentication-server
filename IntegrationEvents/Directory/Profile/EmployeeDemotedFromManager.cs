using System;

namespace IntegrationEvents.Directory.Profile
{
	public class EmployeeDemotedFromManager : IntegrationEvent
	{
		public EmployeeDemotedFromManager(Guid tenantId, Guid employeeId) : base(tenantId)
		{
			EmployeeId = employeeId;
		}

		public Guid EmployeeId { get; set; }

		public override string ToString()
		{
			return $"{nameof(EmployeeDemotedFromManager)} : {Id}";
		}
	}
}