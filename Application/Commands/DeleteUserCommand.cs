using System;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
