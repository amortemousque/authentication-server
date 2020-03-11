using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using AuthorizationServer.Application.Commands;
using AuthorizationServer.Application.Queries;
using AuthorizationServer.Domain.Services;
using AuthorizationServer.Domain.TenantAggregate;
using AuthorizationServer.Exceptions;
using AuthorizationServer.Infrastructure.IdentityServer.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Model = AuthorizationServer.Domain.UserAggregate;
using AuthorizationServer.Models.AccountViewModels;
using AuthorizationServer.Resources;


namespace AuthorizationServer.Controllers
{
    [Authorize]
    public class TestController : Controller
    {

        private readonly UserManager<Model.IdentityUser> _userManager;
        private readonly SignInManager<Model.IdentityUser> _signInManager;
        private readonly IInvitesQueries _invitesQueries;
        private readonly IStringLocalizer<SharedResource> _localizer;

        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly TenantQueries _tenantQueries;
        private readonly IMediator _mediator;


        public TestController(
            UserManager<Model.IdentityUser> userManager,
            SignInManager<Model.IdentityUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILoggerFactory loggerFactory,
            TenantQueries tenantQueries,
            IStringLocalizer<SharedResource> localizer,
            IMediator mediator, IInvitesQueries invitesQueries)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _tenantQueries = tenantQueries;
            _localizer = localizer;
            _mediator = mediator;
            _invitesQueries = invitesQueries;
            _logger = loggerFactory.CreateLogger<TestController>();
        }

        public Tenant GetCurrentTenant()
        {
            var tenantName = HttpContext.Request.GetTenantNameFromHost();
            var tenant = _tenantQueries.GetTenantByNameAsync(tenantName).Result;
            return tenant;
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var tenant = GetCurrentTenant();
                ViewBag.LogoUri = tenant.LogoUri;
            }
            catch (Exception)
            {
                context.Result = StatusCode(404);
            }
        }

        //
        // GET: /Account
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null)
        {
            return RedirectToAction(nameof(AccountController.Login), "Account");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }
    }
}
