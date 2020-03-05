using System;

namespace IntegrationEvents.Directory.Profile
{
	public class EmployeePromotedToDirector : IntegrationEvent
	{
		public EmployeePromotedToDirector(Guid tenantId, Guid employeeId) : base(tenantId)
		{
			EmployeeId = employeeId;
		}

		public Guid EmployeeId { get; set; }

		public override string ToString()
		{
			return $"{nameof(EmployeePromotedToDirector)} : {Id}";
		}
	}
}