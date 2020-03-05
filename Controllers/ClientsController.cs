using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizationServer.Models;
using AuthorizationServer.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MediatR;
using AuthorizationServer.Application.Queries;
using System;
using System.Collections.Generic;
using AuthorizationServer.Application.Commands;
using AuthorizationServer.Domain.ClientAggregate;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientsController : Controller
    {
        protected IMediator _mediator;
        private readonly ClientQueries _clientQueries;

        public ClientsController(
            IMediator mediator,
            ClientQueries clientQueries)
        {
            _mediator = mediator;
            _clientQueries = clientQueries;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Client), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:read")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            try
            {
                var api = await _clientQueries.GetClientAsync(id);
                return Ok(api);
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

        [HttpGet]
        [ProducesResponseType(typeof(Client[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "authentications:read")]
        public async Task<IActionResult> GetClients(string name, bool? enabled)
        {
            try
            {
                var clients = await _clientQueries.GetClientsAsync(name, enabled);
                return Ok(clients);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(Client), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Post([FromBody]CreateClientCommand command)
        {
            try
            {
                var client = await _mediator.Send(command);
                return Ok(client);
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
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateClientCommand command)
        {
            try
            {
                command.Id = id;
                var client = await _mediator.Send(command);
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


        [HttpPut("{id}/clientSecret", Name = "UpdateClientSecret")]
        [ProducesResponseType(typeof(ClientSecret), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> UpdateClientSecret(Guid id, [FromBody] UpdateClientSecretCommand command)
        {
            try
            {
                command.Id = id;
                var clientSecret = await _mediator.Send(command);
                return Ok(clientSecret);
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
        public async Task<IActionResult> Delete(DeleteClientCommand command)
        {
            try
            {
                var client = await _mediator.Send(command);
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
