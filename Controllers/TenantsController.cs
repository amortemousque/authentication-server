using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuthorizationServer.Application.Queries;
using System;
using System.Collections.Generic;
using AuthorizationServer.Application.Commands;
using AuthorizationServer.Domain.TenantAggregate;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TenantsController : Controller
    {
        protected IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;
        private readonly TenantQueries _tenantQueries;

        public TenantsController(
            IAuthorizationService authorizationService,
            IMediator mediator,
            TenantQueries tenantQueries)
        {
            _mediator = mediator;
            _tenantQueries = tenantQueries;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Tenant), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize]
        public async Task<IActionResult> GetTenant(Guid id)
        {
            try
            {
                var userAuthorize = await _authorizationService.AuthorizeAsync(User, "tenants:read");
                var applicationAuthorize = await _authorizationService.AuthorizeAsync(User, "application(tenants:read)");

                if (userAuthorize.Succeeded || applicationAuthorize.Succeeded)
                {
                    var api = await _tenantQueries.GetTenantByIdAsync(id);
                    return Ok(api);
                }
                return StatusCode(401);

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(Tenant[]), 200)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 400)]
        public async Task<IActionResult> GetTenants(string name, bool? enabled)
        {
            try
            {
                var userAuthorize = await _authorizationService.AuthorizeAsync(User, "tenants:read");
                var applicationAuthorize = await _authorizationService.AuthorizeAsync(User, "application(tenants:read)");

                if (userAuthorize.Succeeded || applicationAuthorize.Succeeded)
                {
                    var tenants = await _tenantQueries.GetTenantsAsync(name, enabled);
                    return Ok(tenants);
                }
                return StatusCode(401);
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
        [ProducesResponseType(typeof(Tenant), 201)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 400)]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]CreateTenantCommand command)
        {
            try
            {
                var userAuthorize = await _authorizationService.AuthorizeAsync(User, "tenants:write");
                var applicationAuthorize = await _authorizationService.AuthorizeAsync(User, "application(tenants:write)");

                if(userAuthorize.Succeeded || applicationAuthorize.Succeeded) {
                    var tenant = await _mediator.Send(command);
                    return Ok(tenant);
                }
                return StatusCode(401);
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
        [Authorize(Policy = "tenants:write")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateTenantCommand command)
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
        [Authorize(Policy = "tenants:write")]
        public async Task<IActionResult> Delete(DeleteTenantCommand command)
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
