using Microsoft.AspNetCore.Builder;
using AuthorizationServer.ExceptionHandling.Middlewares;

namespace AuthorizationServer.ExceptionHandling.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseExceptiontMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
    