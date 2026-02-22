using ManaChat.API.Clients;
using ManaChat.API.Models.Auth;
using ManaChat.Core.Models.Auth;
using ManaChat.Identity.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ManaChat.API.Middleware
{
    public class IdentityValidationMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context, IAuthenticatedUserDetails user, IUsersRepository userRepo)
        {
            var authedUser = user as AuthenticatedUserDetails ?? throw new Exception("Authenticated user details is not of type AuthenticatedUserDetails");

            if (IsEndpointAllowAnonymous(context))
            {
                authedUser.IsAuthenticated = false;
                await next(context);
                return;
            }

            var client = ClientFetcher.GetFromHeaders(context.Request.Headers);
            var token = client.GetClientToken(context.Request);
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized", message = "No token provided" });
                return;
            }

            var session = await userRepo.GetUserSession(token);
            if (session.IsTorn)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", message = "Session is torn" });
                return;
            }

            var sessionActual = session.GetValue()!;
            if (sessionActual.ExpiresAt < DateTimeOffset.UtcNow)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized", message = "Session expired" });
                return;
            }

            authedUser.UserId = sessionActual.UserId;
            authedUser.IsAuthenticated = true;

            await next(context);
            return;
        }

        private static bool IsEndpointAllowAnonymous(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor != null)
                {
                    return controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Length > 0;
                }
            }
            return false;
        }
    }
}
