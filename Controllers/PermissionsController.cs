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

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PermissionsController : Controller
    {
        protected IMediator _mediator;
        private readonly PermissionQueries _permissionQueries;

        public PermissionsController(
            IMediator mediator,
            PermissionQueries permissionQueries)
        {
            _mediator = mediator;
            _permissionQueries = permissionQueries;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Permission), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:read")]
        public async Task<IActionResult> GetPermission(Guid id)
        {
            try
            {
                var api = await _permissionQueries.GetPermissionAsync(id);
                return Ok(api);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(Permission[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "permissions:read")]
        public async Task<IActionResult> GetPermissions(string name)
        {
            try
            {
                var permissions = await _permissionQueries.GetPermissionsAsync(name);
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


        [HttpPost]
        [ProducesResponseType(typeof(Permission), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> Post([FromBody]CreatePermissionCommand command)
        {
            try
            {
                var permission = await _mediator.Send(command);
                return Ok(permission);
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
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdatePermissionCommand command)
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
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> Delete(DeletePermissionCommand command)
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

        [HttpDelete( Name = "DeleteMultiple")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "permissions:write")]
        public async Task<IActionResult> DeleteMultiple(Guid[] ids)
        {
            try
            {
                foreach(var id in ids) 
                {
                    var command = new DeletePermissionCommand() { Id = id };
                    await _mediator.Send(command);
                }
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
