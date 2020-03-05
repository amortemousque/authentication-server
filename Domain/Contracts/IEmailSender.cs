using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthorizationServer.Domain.UserAggregate;

namespace AuthorizationServer.Domain.Services
{
    public interface IEmailSender
    {
        Task SendEmailForUserCreation(IdentityUser user, string code);

        Task SendEmailResetPassword(IdentityUser user, string code);

        Task SendEmaiSecurityCode(IdentityUser user, string code);
    }
}
