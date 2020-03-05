using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.RoleAggregate;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class CreateRoleCommand : IRequest<IdentityRole>
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public List<string> Permissions { get; set; }

    }
}
        