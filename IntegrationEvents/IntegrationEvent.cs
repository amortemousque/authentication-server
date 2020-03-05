using System;
namespace IntegrationEvents
{
    public class IntegrationEvent
    {
        public Guid Id { get; }
        public Guid InitiatorId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }

		public IntegrationEvent()
        {
            Id = Guid.NewGuid();
        }

        public IntegrationEvent(Guid tenantId) : this()
        {
	        TenantId = tenantId;
        }
    }
}
