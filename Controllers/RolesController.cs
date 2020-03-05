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

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController : Controller
    {
        protected IMediator _mediator;

        private readonly RoleManager<Model.IdentityRole> _roleManager;
        private readonly RoleQueries _roleQueries;
        private readonly IAuthorizationService _authorizationService;

        public RolesController(
            IMediator mediator,
            IAuthorizationService authorizationService,
            RoleQueries roleQueries,
            RoleManager<Model.IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _mediator = mediator;
            _roleQueries = roleQueries;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Model.IdentityRole), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "roles:read")]
        public async Task<IActionResult> GetRole(Guid id)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(id.ToString());
                if (role == null)
                    throw new KeyNotFoundException();

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
        public async Task<IActionResult> GetRoles(string name)
        {
            try
            {
                var roles = await _roleQueries.GetRolesAsync(name);
                return Ok(roles);
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


        [HttpPost]
        [ProducesResponseType(typeof(Model.IdentityRole), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "roles:write")]
        public async Task<IActionResult> Post([FromBody]CreateRoleCommand command)
        {
            try
            {
                var role = await _mediator.Send(command);
                return Ok(role);
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

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "roles:write")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateRoleCommand command)
        {
            try
            {
                command.Id = id;
                await _mediator.Send(command);
                return Ok();
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


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "roles:write")]
        public async Task<IActionResult> Delete(DeleteRoleCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
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



        // permission

        [HttpGet("{roleId}/permissions", Name = "GetRolePermissions")]
        [ProducesResponseType(typeof(Permission[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:read")]
        public async Task<IActionResult> GetRolePermissions(Guid roleId)
        {
            try
            {
                var permissions = await _roleQueries.GetRolePermissionsAsync(roleId);
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


        [HttpPost("{roleId}/permissions", Name = "PostPermissionRole")]
        [ProducesResponseType(typeof(void), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> PostPermissionRole([FromBody]AddPermissionsToRoleCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
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


        [HttpDelete("{roleId}/permissions/{permissionId}", Name = "DeletePermissionRole")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> DeletePermissionRole(RemovePermissionToRoleCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Ok();
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
    }
}
