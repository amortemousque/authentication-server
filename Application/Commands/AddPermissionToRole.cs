using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class AddPermissionsToRoleCommand : IRequest<bool>
    {
        [Required]
        public Guid RoleId { get; set; }
        [Required]
        public string[] Permissions { get; set; }
    }
}
                