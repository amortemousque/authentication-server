using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.ClientAggregate;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class CreateClientCommand : IRequest<Client>
    {
        [Required]
        public int ClientTypeId { get; set; }
        [Required]
        public string ClientName { get; set; }
    }
}
        