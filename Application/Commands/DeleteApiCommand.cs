using System;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class DeleteApiCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
    