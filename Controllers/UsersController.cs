using System.Threading.Tasks;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
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
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ForbbidenException)
            {
                return StatusCode(403);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(Model.IdentityUser[]), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        public async Task<IActionResult> GetUsers(string name = null, string email = null, List<string> ids = null, string fields = null)
        {
            try
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
        [ProducesResponseType(typeof(Model.IdentityUser), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 409)]
        public async Task<IActionResult> Post([FromBody]CreateUserCommand command)
        {
            try
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
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ForbbidenException)
            {
                return StatusCode(403);
            }
	        catch (ConflictException e)
	        {
		        return Conflict(e.Message);
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
        [Authorize(Policy = "users:write")]
        public async Task<IActionResult> Put(Guid id, [FromBody] UpdateUserCommand command)
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
            catch (ForbbidenException)
            {
                return StatusCode(403);
            }
            catch (ArgumentException argumentException)
            {
                return BadRequest(argumentException.Message);
            }
        }


        [HttpPut("{id}/password", Name = "UpdateUserPassword")]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(void), 401)]
        [ProducesResponseType(typeof(void), 404)]
        [Authorize(Policy = "authentications:write")]
        public async Task<IActionResult> UpdateUserPassword(Guid id, [FromBody] UpdateUserPasswordCommand command)
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
        [Authorize(Policy = "users:write")]
        public async Task<IActionResult> Delete(DeleteUserCommand command)
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
