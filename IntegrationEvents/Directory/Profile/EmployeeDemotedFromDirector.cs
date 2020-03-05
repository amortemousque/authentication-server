using System;

namespace IntegrationEvents.Directory.Profile
{
	public class EmployeeDemotedFromDirector : IntegrationEvent
	{
		public EmployeeDemotedFromDirector(Guid tenantId, Guid employeeId) : base(tenantId)
		{
			EmployeeId = employeeId;
		}

		public Guid EmployeeId { get; set; }

		public override string ToString()
		{
			return $"{nameof(EmployeeDemotedFromDirector)} : {Id}";
		}
	}
}