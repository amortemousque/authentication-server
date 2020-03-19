using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuthorizationServer.Application.Queries;
using System;
using System.Collections.Generic;
using AuthorizationServer.Application.Commands;
using AuthorizationServer.Domain.PermissionAggregate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthorizationServer.Infrastructure.Context;
using AuthorizationServer.Infrastructure;

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize]
    public class PermissionsController : Controller
    {
        protected IMediator _mediator;
        private readonly PermissionQueries _permissionQueries;
        private readonly IIdentityService _identityService;

        public PermissionsController(
            IMediator mediator,
            PermissionQueries permissionQueries,
            IIdentityService identityService)
        {
            _mediator = mediator;
            _permissionQueries = permissionQueries;
            _identityService = identityService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Permission), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:read")]
        public async Task<IActionResult> GetPermission([FromRoute]Guid id)
        {
            var api = await _permissionQueries.GetPermissionAsync(id);
            return Ok(api);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Permission[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "permissions:read")]
        public async Task<IActionResult> GetPermissions([FromQuery]string name)
        {
            var permissions = await _permissionQueries.GetPermissionsAsync(name);
            return Ok(permissions);
        }


        [HttpGet("/users/me/permissions")]
        [ProducesResponseType(typeof(Permission[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "permissions:read")]
        public async Task<IActionResult> GetMyPermissions()
        {
            var permissions = await _permissionQueries.GetUserPermissions(_identityService.GetUserIdentity());
            return Ok(permissions);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Permission), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> Post([FromBody]CreatePermissionCommand command)
        {
            var permission = await _mediator.Send(command);
            return Ok(permission);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody] UpdatePermissionCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return Ok();
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var command = new DeletePermissionCommand() { Id = id };
            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete(Name = "DeleteMultiple")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> DeleteMultiple([FromQuery]Guid[] ids)
        {
            foreach (var id in ids)
            {
                var command = new DeletePermissionCommand() { Id = id };
                await _mediator.Send(command);
            }
            return Ok();
        }
    }
}
