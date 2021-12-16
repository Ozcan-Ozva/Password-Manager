using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.WebApiCompatShim;

namespace InformationSecurity.Filters
{
    public class RequestHeaderMACFilter : Attribute, IAsyncActionFilter
    {
        private readonly string authenticationScheme = "hmacauth";

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            HttpRequestMessageFeature hreqmf = new HttpRequestMessageFeature(context.HttpContext);
            HttpRequestMessage req = hreqmf.HttpRequestMessage;

            if (req.Headers.Authorization == null )
            {
                context.Result = new JsonResult(new
                {
                    Message = "Token Validation Has Failed. Request Access Denied"
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else
            {
                if (!(authenticationScheme.Equals(req.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Result = new JsonResult(new
                    {
                        Message = "Token Validation Has Failed. Request Access Denied"
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
            }
            return Task.CompletedTask;
        }
    }
}
