using System;
using System.Text.RegularExpressions;

namespace AuthorizationServer.Infrastructure.IdentityServer
{
    public static class WildcardUrlHelper
    {
    
        public static bool Compare(string wildcard, string url) 
        {
            if (wildcard.Contains("*"))
            {
                var escapeUri = wildcard.Replace("/", @"\/").Replace(".", @"\.");
                var pattern = $"^{escapeUri.Replace("*", @"\w+")}+$";
                if (Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }
            else if (wildcard == url)
            {
                return true;
            }

            return false;
        }
    }
}
