using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace PetServiceManagement.API.Filters
{
    public class CookieActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {

            if (context.HttpContext.Request.Cookies.ContainsKey("X-Access-Token-Admin"))
            {
                var cookie = context.HttpContext.Request.Cookies["X-Access-Token-Admin"];

                context.HttpContext.Response.Cookies.Append("X-Access-Token-Admin", cookie, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true, Path = "/", Expires = DateTime.Now.AddMinutes(30) });
            }
        }
    }
}
