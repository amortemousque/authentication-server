using System;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class DeleteClientCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
