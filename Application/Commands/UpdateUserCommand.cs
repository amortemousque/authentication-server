using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        [Required]
        public string GivenName { get; set; }
        [Required]
        public string FamilyName { get; set; }
        public string Password { get; set; }
    }
}
        