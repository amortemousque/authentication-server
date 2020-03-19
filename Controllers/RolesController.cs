using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuthorizationServer.Application.Queries;
using System;
using System.Collections.Generic;
using AuthorizationServer.Application.Commands;
using Model = AuthorizationServer.Domain.RoleAggregate;
using AuthorizationServer.Domain.PermissionAggregate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthorizationServer.Exceptions;

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize]
    public class RolesController : Controller
    {
        protected IMediator _mediator;

        private readonly RoleManager<Model.IdentityRole> _roleManager;
        private readonly RoleQueries _roleQueries;
        private readonly PermissionQueries _permissionQueries;

        private readonly IAuthorizationService _authorizationService;

        public RolesController(
            IMediator mediator,
            IAuthorizationService authorizationService,
            RoleQueries roleQueries,
            PermissionQueries permissionQueries,
            RoleManager<Model.IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _mediator = mediator;
            _roleQueries = roleQueries;
            _permissionQueries = permissionQueries;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Model.IdentityRole), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "roles:read")]
        public async Task<IActionResult> GetRole([FromRoute]Guid id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null)
                    throw new NotFoundException();

                return Ok(role);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(Model.IdentityRole[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "roles:read")]
        public async Task<IActionResult> GetRoles([FromQuery]string name)
        {
            var roles = await _roleQueries.GetRolesAsync(name);
            return Ok(roles);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Model.IdentityRole), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "roles:write")]
        public async Task<IActionResult> Post([FromBody]CreateRoleCommand command)
        {
            var role = await _mediator.Send(command);
            return Ok(role);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "roles:write")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateRoleCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return Ok();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "roles:write")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteRoleCommand { Id = id };
            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("{id}/permissions", Name = "GetRolePermissions")]
        [ProducesResponseType(typeof(Permission[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:read")]
        public async Task<IActionResult> GetRolePermissions([FromRoute]Guid id)
        {
            try
            {
                var permissions = await _permissionQueries.GetRolePermissionsAsync(id);
                return Ok(permissions);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(argumentException.Message);
            }
        }


        [HttpPost("{id}/permissions", Name = "PostPermissionRole")]
        [ProducesResponseType(typeof(void), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> PostPermissionRole([FromRoute]Guid id, [FromBody] AddPermissionsToRoleCommand command)
        {
            command.RoleId = id;
            await _mediator.Send(command);
            return Ok();
        }


        [HttpDelete("{id}/permissions/{permissionId}", Name = "DeletePermissionRole")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> DeletePermissionRole([FromRoute]Guid id, [FromRoute]Guid permissionId)
        {
            var command = new RemovePermissionToRoleCommand { RoleId = id, PermissionId = permissionId };
            await _mediator.Send(command);
            return Ok();
        }
    }
}
