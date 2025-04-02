using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebApplication3.Controllers
{
    public class SessionCheckAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var sessionEmail = context.HttpContext.Session.GetString("email");
            var currentAction = context.RouteData.Values["action"]?.ToString();
            var currentController = context.RouteData.Values["controller"]?.ToString();

            // Prevent redirect loop
            if (string.IsNullOrEmpty(sessionEmail) &&
                !(currentController == "Admin" && currentAction == "Login"))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary { { "controller", "Admin" }, { "action", "Login" } });
            }

            base.OnActionExecuting(context);
        }
    }

}

