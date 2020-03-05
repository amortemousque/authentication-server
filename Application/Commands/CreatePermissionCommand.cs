using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AuthorizationServer.Domain.PermissionAggregate;
using MediatR;

namespace AuthorizationServer.Application.Commands
{
    public class CreatePermissionCommand : IRequest<Permission>
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
            