using System;
using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class UpdateTenantCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public string LogoUri { get; set; }

        public string EmailSignature { get; set; }
    }
}
    