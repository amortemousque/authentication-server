using System;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class DeletePermissionCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
        