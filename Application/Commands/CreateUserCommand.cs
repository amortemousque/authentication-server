using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.UserAggregate;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class CreateUserCommand : IRequest<IdentityUser>
    {
        public Guid? TenantId { get; set; }

        [Required]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Password { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public string Culture { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        
        [JsonIgnore]
        public List<string> Roles { get; set; }
    }
}
            