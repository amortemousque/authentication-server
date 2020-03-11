
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Model = AuthorizationServer.Domain.UserAggregate;

namespace AuthorizationServer.Infrastructure.Managers
{
    public class CustomSignInManager : SignInManager<Model.IdentityUser>
    {
    
        private readonly UserManager<Model.IdentityUser> _userManager;

        public CustomSignInManager(UserManager<Model.IdentityUser> userManager, 
                                   IHttpContextAccessor contextAccessor, 
                                   IUserClaimsPrincipalFactory<Model.IdentityUser> claimsFactory, 
                                   IOptions<IdentityOptions> optionsAccessor, 
                                   ILogger<SignInManager<Model.IdentityUser>> logger, 
                                   IAuthenticationSchemeProvider schemes,
                                   IUserConfirmation<Model.IdentityUser> confirmations)
            : base(userManager,
                   contextAccessor,
                   claimsFactory,
                   optionsAccessor,
                   logger,
                   schemes,
                   confirmations)
        {
            if (userManager == null)
                throw new ArgumentNullException(nameof(userManager));

            _userManager = userManager;
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var result = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            if(result.Succeeded) {
                user.LoggedInAt = DateTime.Now;
                await UserManager.UpdateAsync(user);
            }
            return result;
        }

        public override async Task SignInAsync(Model.IdentityUser user, bool isPersistent, string authenticationMethod = null)
        {
            await base.SignInAsync(user, isPersistent, authenticationMethod);
            user.LoggedInAt = DateTime.Now;
            await UserManager.UpdateAsync(user);
        }

        public override async Task SignInAsync(Model.IdentityUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null) 
        {
            await base.SignInAsync(user, authenticationProperties, authenticationMethod);
            user.LoggedInAt = DateTime.Now;
            await UserManager.UpdateAsync(user);
        }

    }
}