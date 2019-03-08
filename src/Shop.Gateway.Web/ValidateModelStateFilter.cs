using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shop.Gateway.Web
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
                return;
            var firstError = context.ModelState.Keys.SelectMany(k => context.ModelState[k].Errors).Select(e => e.ErrorMessage).LastOrDefault();
            context.Result = new BadRequestObjectResult(firstError);
        }
    }
}
