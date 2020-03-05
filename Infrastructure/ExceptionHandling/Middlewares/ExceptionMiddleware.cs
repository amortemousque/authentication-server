using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using AuthorizationServer.ExceptionHandling.Models;
using AuthorizationServer.Exceptions;

namespace AuthorizationServer.ExceptionHandling.Middlewares
{
    public class ExceptionMiddleware
    {
        readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            string param = null;
            string message = exception.Message;

            if (exception is NotFoundException) 
            { 
                code = HttpStatusCode.NotFound; 
            }
            else if (exception is ForbbidenException) 
            { 
                code = HttpStatusCode.Forbidden; 
            }
            else if (exception is ConflictException) 
            { 
                code = HttpStatusCode.Conflict; 
            }
            else if (exception is DomainException) 
            { 
                code = HttpStatusCode.BadRequest; 
            }
            else if (exception is ArgumentNullException) 
            {
                code = HttpStatusCode.BadRequest;
                param = (exception as ArgumentNullException).ParamName;
                message = exception.Message ?? $"{param} is required";
            }
            else if (exception is ArgumentException) 
            { 
                code = HttpStatusCode.BadRequest; 
                param = (exception as ArgumentException).ParamName;
                message = exception.Message ?? $"Invalid argument {param}";
            }

            var errors = new List<Error> 
            {
                new Error(param, message)
            };

            var result = JsonConvert.SerializeObject(new ErrorResult(message, code, errors));

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
