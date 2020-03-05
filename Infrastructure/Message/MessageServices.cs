using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntegrationEvents.User.Account;
using IntegrationEvents.User.Commands;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.Services;
using AuthorizationServer.Domain.UserAggregate;
using AuthorizationServer.Infrastructure.SharedResources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;

namespace AuthorizationServer.Infrastructure.Message
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _accessor;
        private readonly IUrlHelper _urlHelper;
        private readonly ISharedResource _sharedResource;
        private readonly ITenantRepository _repository;


        public AuthMessageSender(IBus bus, IHttpContextAccessor accessor, IUrlHelper urlHelper, ISharedResource sharedResource, ITenantRepository repository) 
        {
            _bus = bus;
            _accessor = accessor;
            _urlHelper = urlHelper;
            _sharedResource = sharedResource;
            _repository = repository;
        }

        private string ReplaceTenantDomaine(string url, string tenantName) 
        {
            return Regex.Replace(url, @"\:\/\/[a-z0-9]+", "://" + tenantName);
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            await _bus.Send(new SendEmail(new List<EmailRecipient> { new EmailRecipient{ Address = email } }, subject, message, message));
        }

        public async Task SendEmailForUserCreation(IdentityUser user, string code)
        {
            var callbackUrl = _urlHelper.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: _accessor.HttpContext.Request.Scheme);
            var tenantName = (await _repository.GetById(user.TenantId)).Name;
            callbackUrl = ReplaceTenantDomaine(callbackUrl, tenantName);
            await this.SendEmailAsync(user.Email,
                                      _sharedResource.GetResourceValueByKey("Confirm your account."),
                                      _sharedResource.GetResourceValueByKey("Please confirm your account by clicking this link: {0}", " <a href=\"" + callbackUrl + "\">link</a>"));
        }


        public async Task SendEmailResetPassword(IdentityUser user, string code)
        {
            var callbackUrl = _urlHelper.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: _accessor.HttpContext.Request.Scheme);
            var tenantName = (await _repository.GetById(user.TenantId)).Name;
            callbackUrl = ReplaceTenantDomaine(callbackUrl, tenantName);
            var resetRequested = new ResetPasswordRequested { ResetPasswordLink = callbackUrl, TenantName = tenantName, TenantId = user.TenantId,
	            InitiatorId = new Guid(user.Id) };
            await _bus.Publish(resetRequested);
        }

        public async Task SendEmaiSecurityCode(IdentityUser user, string code)
        {
            var message = "Your security code is: " + code;
            await this.SendEmailAsync(user.Email, _sharedResource.GetResourceValueByKey("Security Code"), message);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
