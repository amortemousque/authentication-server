using System;
using System.Collections.Generic;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class DeleteRoleCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
