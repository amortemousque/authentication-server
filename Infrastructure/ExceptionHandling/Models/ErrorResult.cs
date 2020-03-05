using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace AuthorizationServer.ExceptionHandling.Models
{
    public class ErrorResult
    {
        public string Message { get; }

        public HttpStatusCode Code { get; }

        public List<Error> Errors { get; }

        public ErrorResult(string message, HttpStatusCode code, List<Error> errors = null)
        {
            Message = message;
            Code = code;
            Errors = errors;
        }

        public ErrorResult(ModelStateDictionary modelState)
        {
            Message = "Validation Failed";
            Code = HttpStatusCode.BadRequest;
            Errors = modelState.Keys
                    .SelectMany(key => modelState[key].Errors.Select(x => new Error(key, x.ErrorMessage)))
                    .ToList();
        }
    }
}
        