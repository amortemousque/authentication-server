using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;

namespace AuthorizationServer.Extensions
{
    //
    // Résumé :
    //     A helper class for constructing encoded Uris for use in headers and other Uris.
    public static class IConfigurationExtensions
    {

        public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        {
            T value = configuration.GetValue<T>(key);
            if (typeof(T) == typeof(string) && string.IsNullOrWhiteSpace(value.ToString()))
            {
                throw new Exception("Configuration variable " + key + " not found. You must add it in your docker environment config or appsettings.");
            }
            else if (value == null)
            {
                throw new Exception("Configuration variable " + key + " not found. You must add it in your docker environment config or appsettings.");
            }
            return value;      
        }

        public static string GetRequiredConnectionString(this IConfiguration configuration, string key)
        {
            var value = configuration.GetConnectionString(key) ?? throw new Exception("Configuration variable " + key + " not found. You must add it in your docker environment config or appsettings.");
            return value;
        }

    }

}    