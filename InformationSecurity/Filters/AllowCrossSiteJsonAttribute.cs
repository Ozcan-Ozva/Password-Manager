using Microsoft.AspNetCore.Mvc.Filters;

namespace InformationSecurity.Filters
{
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

            base.OnActionExecuting(filterContext);
        }
    }
}
