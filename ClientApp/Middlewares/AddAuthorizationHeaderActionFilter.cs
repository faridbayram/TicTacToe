using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ClientApp.Middlewares
{
    public class AuthHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Session.TryGetValue("AuthToken", out var tokenBytes))
            {
                context.Request.Headers.Add("Authorization", $"Bearer {Encoding.UTF8.GetString(tokenBytes)}");
            }
            await _next(context);
        }
    }
}