using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IntegrationEvents.User.Account;
using IntegrationEvents.User.Commands;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.Services;
using AuthorizationServer.Domain.UserAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rebus.Bus;
using AuthorizationServer.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Routing;

namespace AuthorizationServer.Infrastructure.Message
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly IBus _bus;
        private readonly IHttpContextAccessor _accessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ITenantRepository _repository;


        public AuthMessageSender(IBus bus, IHttpContextAccessor accessor, LinkGenerator linkGenerator, IStringLocalizer<SharedResource> localizer, ITenantRepository repository) 
        {
            _bus = bus;
            _accessor = accessor;
            _linkGenerator = linkGenerator;
            _localizer = localizer;
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
            var callbackUrl = _linkGenerator.GetPathByAction("ConfirmEmail", "Account", new { userId = user.Id, code = code });
            var tenantName = (await _repository.GetById(user.TenantId)).Name;
            callbackUrl = ReplaceTenantDomaine(callbackUrl, tenantName);
            await SendEmailAsync(user.Email,
                                      _localizer.GetString("Confirm your account."),
                                      _localizer.GetString("Please confirm your account by clicking this link: {0}", " <a href=\"" + callbackUrl + "\">link</a>"));
        }


        public async Task SendEmailResetPassword(IdentityUser user, string code)
        {
            var callbackUrl = _linkGenerator.GetPathByAction("ResetPassword", "Account", new { userId = user.Id, code = code });
            var tenantName = (await _repository.GetById(user.TenantId)).Name;
            callbackUrl = ReplaceTenantDomaine(callbackUrl, tenantName);
            var resetRequested = new ResetPasswordRequested { ResetPasswordLink = callbackUrl, TenantName = tenantName, TenantId = user.TenantId,
	            InitiatorId = new Guid(user.Id) };
            await _bus.Publish(resetRequested);
        }

        public async Task SendEmaiSecurityCode(IdentityUser user, string code)
        {
            var message = "Your security code is: " + code;
            await SendEmailAsync(user.Email, _localizer.GetString("Security Code"), message);
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
