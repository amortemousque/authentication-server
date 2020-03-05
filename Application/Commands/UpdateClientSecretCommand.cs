using System;
using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.ClientAggregate;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class UpdateClientSecretCommand : IRequest<ClientSecret>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Mode { get; set; } = "reset";
    }
}
