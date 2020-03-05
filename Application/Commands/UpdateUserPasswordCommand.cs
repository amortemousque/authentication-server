using System;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class UpdateUserPasswordCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Mode { get; set; } = "reset";
    }
}
