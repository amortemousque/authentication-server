using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using AuthorizationServer.Domain.UserAggregate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using AuthorizationServer.Infrastructure.IdentityServer.Extensions;
using IdentityModel;
using IdentityUser = AuthorizationServer.Domain.UserAggregate.IdentityUser;

public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{

    public UserManager<IdentityUser> UserManager { get; }

    public CustomResourceOwnerPasswordValidator(UserManager<IdentityUser> um)
    {
        UserManager = um;
    }


    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await UserManager.FindByNameAsync(context.UserName);

        if (user == null)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Username or password is incorrect");
            return;
        }

        var passwordValid = await UserManager.CheckPasswordAsync(user, context.Password);
        if (!passwordValid)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Username or password is incorrect");
            return;

        }

        context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password);
    }
}