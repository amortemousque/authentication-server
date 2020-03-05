using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class UpdateApiCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
    