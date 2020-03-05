using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class UpdateRoleCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public List<string> Permissions { get; set; }

    }
}
            