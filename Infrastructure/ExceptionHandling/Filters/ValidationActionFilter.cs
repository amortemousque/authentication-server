using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AuthorizationServer.ExceptionHandling.Models;

namespace AuthorizationServer.ExceptionHandling.Filters
{
    public class ValidationActionFilter: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var result = new ErrorResult(context.ModelState);
                context.Result = new BadRequestObjectResult(result);
            }
        }
    }
}    