using System.Threading.Tasks;
using MediatR;
using System;
using System.Threading;
using AuthorizationServer.Application.Commands;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using AuthorizationServer.Domain.Contracts;
using IdentityRole = AuthorizationServer.Domain.RoleAggregate.IdentityRole;
using AuthorizationServer.Exceptions;

namespace AuthorizationServer.Application.CommandHandlers
{
    public class RoleHandlers :
    IRequestHandler<CreateRoleCommand, IdentityRole>,
    IRequestHandler<UpdateRoleCommand, bool>,
    IRequestHandler<DeleteRoleCommand, bool>,
    IRequestHandler<AddPermissionsToRoleCommand, bool>,
    IRequestHandler<RemovePermissionToRoleCommand, bool>

    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleRepository _roleRepository;

        private readonly IPermissionRepository _permissionRepository;

        public RoleHandlers(
            RoleManager<IdentityRole> roleManager,
            IPermissionRepository permissionRepository,
            IRoleRepository roleRepository

        )
        {
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _permissionRepository = permissionRepository;
        }

        public async Task<IdentityRole> Handle(CreateRoleCommand message, CancellationToken cancellationToken)
        {
            var role = new IdentityRole
            {
                Name = message.Name,
                Description = message.Description,
                Permissions = message.Permissions
            };

                var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                if (result.Errors.First().Code == "DuplicateRoleName")
                {
                    throw new ArgumentException(result.Errors.First().Description, "name");
                }
            }

            return role;
        }

        public async Task<bool> Handle(UpdateRoleCommand message, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(message.Id.ToString());
            role.Description = message.Description;
            role.Permissions = message.Permissions;

            await _roleManager.UpdateAsync(role);

            return true;
        }

        public async Task<bool> Handle(DeleteRoleCommand message, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(message.Id.ToString());

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                throw new ArgumentException(result.Errors.First().Description, nameof(role));
            }

            return true;
        }

        public async Task<bool> Handle(AddPermissionsToRoleCommand message, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(message.RoleId.ToString()) ?? throw new NotFoundException();;
            var permissions = await _permissionRepository.GetByNames(message.Permissions) ?? throw new NotFoundException();

            role.Permissions = role.Permissions ?? new List<string>();

            foreach(var permission in permissions) 
            {
                if(!role.Permissions.Any(pname => pname == permission.Name)) 
                {
                    role.Permissions.Add(permission.Name);
                }
            }
                
            await _roleManager.UpdateAsync(role);
            return true;
        }

        public async Task<bool> Handle(RemovePermissionToRoleCommand message, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(message.RoleId.ToString()) ?? throw new NotFoundException();
            var permission = await _permissionRepository.GetById(message.PermissionId) ?? throw new NotFoundException();

            if (role.Permissions.Any(pname => pname == permission.Name)) 
            {
                role.Permissions.Remove(permission.Name);
            } 
            else 
            {
                throw new NotFoundException("Permission not found in this role");
            }

                
            await _roleManager.UpdateAsync(role);
            return true;
        }


    }
}