using System;
using System.Collections.Generic;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class RemovePermissionToRoleCommand : IRequest<bool>
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
                    