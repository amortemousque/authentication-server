using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuthorizationServer.Application.Queries;
using System;
using System.Collections.Generic;
using AuthorizationServer.Application.Commands;
using AuthorizationServer.Domain.ApiAggregate;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ApisController : Controller
    {
        protected IMediator _mediator;
        private readonly ApiQueries _apiQueries;

        public ApisController(
            IMediator mediator,
            ApiQueries apiQueries)
        {
            _mediator = mediator;
            _apiQueries = apiQueries;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResource), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        public async Task<IActionResult> GetApi(Guid id)
        {
            var api = await _apiQueries.GetApiAsync(id);
            return Ok(api);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResource[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "authentications:read")]
        public async Task<IActionResult> GetApis([FromQuery]string name, [FromQuery]string displayName, [FromQuery]bool? enabled)
        {
            try
            {
                var apis = await _apiQueries.GetApisAsync(name, displayName, enabled);
                return Ok(apis); 
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(argumentException.Message);
            }
          
        }


        [HttpPost]
        [ProducesResponseType(typeof(ApiResource), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Post([FromBody]CreateApiCommand command)
        {
            var apiResource = await _mediator.Send(command);
            return Ok(apiResource);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody]UpdateApiCommand command)
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
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Delete([FromRoute]Guid id)
        {
            var command = new DeleteApiCommand { Id = id };
            await _mediator.Send(command);
            return Ok();
        }


        //scope

        [HttpGet("{id}/scopes", Name = "GetApiScopes")]
        [ProducesResponseType(typeof(ApiResource[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:read")]
        public async Task<IActionResult> GetApiScopes([FromRoute]Guid id)
        {
            var scopes = await _apiQueries.GetApiScopesAsync(id);
            return Ok(scopes);
        }


        [HttpPost("{id}/scopes", Name = "PostApiScope")]
        [ProducesResponseType(typeof(ApiResource), 201)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 400)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> PostApiScope([FromRoute] Guid id, [FromBody]CreateApiScopeCommand command)
        {
            try
            {
                command.ApiResourceId = id;
                var apiResource = await _mediator.Send(command);
                return Ok(apiResource);
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


        [HttpPut("{id}/scopes/{scopeId}", Name = "PutApiScope")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> PutApiScope([FromRoute]Guid id, [FromRoute]Guid scopeId, [FromBody]UpdateApiScopeCommand command)
        {
            command.ApiResourceId = id;
            command.Id = scopeId;

            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("{id}/scopes/{scopeId}", Name = "DeleteApiScope")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> DeleteApiScope([FromRoute]Guid id, [FromRoute]Guid scopeId)
        {
            var command = new DeleteApiScopeCommand { Id = scopeId, ApiResourceId = id };
            await _mediator.Send(command);
            return Ok();
        }
    }
}
    