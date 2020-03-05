using System;
using MediatR;

namespace AuthorizationServer.Application.Events
{
    public class PermissionDeletedEvent : INotification
    {
        public Guid PermissionId { get; set; }

        public string Name { get; set; }

        public PermissionDeletedEvent(Guid id, string name)
        {
            PermissionId = id;
            Name = name;
        }
    }
}
    