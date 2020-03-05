using System;
namespace IntegrationEvents
{
    public class UserDeletedEvent
    {

        public UserDeletedEvent()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public override string ToString()
        {
            return $"Message1 : {Id}";
        }

        public Guid UserId { get; set; }

        public Guid TenantId { get; set; }
    }
}
