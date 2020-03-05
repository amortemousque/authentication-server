using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
 
using AuthorizationServer.Application.Events;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.RoleAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using IdentityRole = AuthorizationServer.Domain.RoleAggregate.IdentityRole;

namespace AuthorizationServer.Application.EventHandlers
{
    public class DetachPermissionFromRole : INotificationHandler<PermissionDeletedEvent>
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IPermissionRepository _permissionRepository;

        public DetachPermissionFromRole(RoleManager<IdentityRole> roleManager,
                                      IPermissionRepository permissionRepository)
        {
            _roleManager = roleManager;
            _permissionRepository = permissionRepository;
        }

        public async Task Handle(PermissionDeletedEvent notification, CancellationToken cancellationToken)
        {
            //Delete permission reference into roles
            var roles = _roleManager.Roles.ToList().Where(role => role.Permissions != null && role.Permissions.Any(pname => pname == notification.Name)).ToList();

            foreach(var role in roles) 
            {
                role.Permissions.Remove(notification.Name);
                await _roleManager.UpdateAsync(role);
            }
        }
    }
}
        