using System;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class DeleteApiScopeCommand : IRequest<bool>
    {
        public Guid ApiResourceId { get; set; }
        public Guid Id { get; set; }
    }
}
    