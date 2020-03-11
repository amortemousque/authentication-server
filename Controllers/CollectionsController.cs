using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizationServer.Application.Queries;
using Microsoft.Extensions.Localization;
using AuthorizationServer.Domain;
using AuthorizationServer.Domain.SeedWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AuthorizationServer.Resources;

namespace AuthorizationServer.Controllers
{
    [ApiVersion("1")]
    [ApiController]
    [Produces("application/json")]
    [Route("v{version:apiVersion}/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CollectionsController : Controller
    {
        
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ApiQueries _apiQueries;
        private readonly IdentityResourceQueries _identityResourceQueries;

        public CollectionsController(
            IStringLocalizer<SharedResource> localizer, ApiQueries apiQueries, IdentityResourceQueries identityResourceQueries)
        {
            _localizer = localizer;
            _apiQueries = apiQueries;
            _identityResourceQueries = identityResourceQueries;
        }

        [HttpGet]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [Route("ClientTypes")]
        [Authorize(Policy = "authentications:read")]
        public async Task<IActionResult> GetClientTypes()
        {
            var types = Enumeration.GetAll<ClientType>().ToList();
            types.ForEach(industry => industry.Name = _localizer[industry.Name]);
            return Ok(types);
        }

        [HttpGet]
        [ProducesResponseType(typeof(void), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [Route("AllowedScopes")]
        [Authorize(Policy = "authentications:read")]
        public async Task<IActionResult> GetAllowedScopes()
        {
            var apiScopes = await _apiQueries.GetAllApiScopesAsync();
            var identityScope = await _identityResourceQueries.GetAllIdentityScopesAsync();
            var allowedScopes =  identityScope.Select(s => new { s.Name }).ToList().Concat(apiScopes.Select(s => new { s.Name }).ToList());

            return Ok(allowedScopes);
        }
    }
}
