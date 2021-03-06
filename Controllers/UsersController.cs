﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuthorizationServer.Application.Queries;
using System;
using System.Collections.Generic;
using AuthorizationServer.Application.Commands;
using AuthorizationServer.Application.Security;
using AuthorizationServer.Exceptions;
using Model = AuthorizationServer.Domain.UserAggregate;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize]
    public class UsersController : Controller
    {
        protected IMediator _mediator;

        private readonly UserManager<Model.IdentityUser> _userManager;
        private readonly SignInManager<Model.IdentityUser> _signInManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserQueries _userQueries;
        private SecurityService _securityService;

        public UsersController(
            IMediator mediator,
            UserQueries userQueries,
            UserManager<Model.IdentityUser> userManager,
            IAuthorizationService authorizationService,
            SignInManager<Model.IdentityUser> signInManager, SecurityService securityService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _securityService = securityService;
            _mediator = mediator;
            _userQueries = userQueries;
            _authorizationService = authorizationService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Model.IdentityUser), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute]Guid id)
        {
            
            var userAuthorize = await _authorizationService.AuthorizeAsync(User, "users:read");
            var applicationAuthorize = await _authorizationService.AuthorizeAsync(User, "application(users:read)");

            if (userAuthorize.Succeeded || applicationAuthorize.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                    throw new KeyNotFoundException();
                if (!await _securityService.CanReadUser(id, user.TenantId)) return StatusCode(403);

                return Ok(user);
            }

            return StatusCode(401);
        }

        [HttpGet]
        [ProducesResponseType(typeof(Model.IdentityUser[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        public async Task<IActionResult> GetUsers([FromQuery]string name = null, [FromQuery]string email = null, [FromQuery]List<string> ids = null, [FromQuery]string fields = null)
        {
            var userAuthorize = await _authorizationService.AuthorizeAsync(User, "users:read");
            var applicationAuthorize = await _authorizationService.AuthorizeAsync(User, "application(users:read)");

            if (userAuthorize.Succeeded || applicationAuthorize.Succeeded)
            {
                var users = await _userQueries.GetUsersAsync(name, email, ids, fields);
                return Ok(users);
            }
            return StatusCode(401);
        }


        [HttpPost]
        [ProducesResponseType(typeof(Model.IdentityUser), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 409)]
        public async Task<IActionResult> Post([FromBody]CreateUserCommand command)
        {
            var userAuthorize = await _authorizationService.AuthorizeAsync(User, "users:write");
            var applicationAuthorize = await _authorizationService.AuthorizeAsync(User, "application(users:write)");

            if (userAuthorize.Succeeded || applicationAuthorize.Succeeded)
            {
                var user = await _mediator.Send(command);
                return Ok(user);
            }

            return StatusCode(401);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "users:write")]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody] UpdateUserCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return Ok();
        }


        [HttpPut("{id}/password", Name = "UpdateUserPassword")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> UpdateUserPassword([FromRoute]Guid id, [FromBody] UpdateUserPasswordCommand command)
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
        [Authorize(Policy = "users:write")]
        public async Task<IActionResult> Delete([FromRoute]DeleteUserCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}
