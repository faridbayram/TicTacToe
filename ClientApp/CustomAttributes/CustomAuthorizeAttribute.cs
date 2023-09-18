using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClientApp.CustomAttributes;

public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var request = context.HttpContext.Request;
        string redirectUrl = $"{request.Path}{request.QueryString}";
        
        var user = context.HttpContext.User;

        if (!user.Identity.IsAuthenticated)
        {
            context.Result = new RedirectToActionResult("login", "account", new {redirectUrl});
            return;
        }

        if (!string.IsNullOrEmpty(Roles))
        {
            var roles = Roles.Split(',');

            if (!roles.Any(a => user.IsInRole(a.Trim())))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);  // 403 Forbidden
                return;
            }
        }

        context.Result = new ViewResult();
    }
}