using System;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Linq;
using System.Text.RegularExpressions;

namespace AuthorizationServer.Infrastructure.IdentityServer
{
    public class CustomRedirectUriValidator : IRedirectUriValidator
    {
        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client.RedirectUris != null && client.RedirectUris.Any())
            {
                foreach (var uri in client.RedirectUris)
                {
                    if (WildcardUrlHelper.Compare(uri, requestedUri))
                    {
                        return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (client.RedirectUris != null && client.RedirectUris.Any()) 
            {
                foreach(var uri in client.RedirectUris) 
                {
                    if (WildcardUrlHelper.Compare(uri, requestedUri)) 
                    {
                        return Task.FromResult(true);
                    }
                }
            }

           return Task.FromResult(false);
        }
    }
}
