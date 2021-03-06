﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize]
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
        public async Task<IActionResult> GetClient([FromRoute]Guid id)
        {
            var api = await _clientQueries.GetClientAsync(id);
            return Ok(api);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Client[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [Authorize(Policy = "authentications:read")]
        public async Task<IActionResult> GetClients([FromQuery]string name, [FromQuery]bool? enabled)
        {
            var clients = await _clientQueries.GetClientsAsync(name, enabled);
            return Ok(clients);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Client), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Post([FromBody]CreateClientCommand command)
        {
            var client = await _mediator.Send(command);
            return Ok(client);
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody]UpdateClientCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return Ok();
        }


        [HttpPut("{id}/clientSecret", Name = "UpdateClientSecret")]
        [ProducesResponseType(typeof(ClientSecret), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> UpdateClientSecret([FromRoute]Guid id, [FromBody] UpdateClientSecretCommand command)
        {
            command.Id = id;
            var clientSecret = await _mediator.Send(command);
            return Ok(clientSecret);
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var command = new DeleteClientCommand { Id = id };
            var client = await _mediator.Send(command);
            return Ok();
        }
    }
}
