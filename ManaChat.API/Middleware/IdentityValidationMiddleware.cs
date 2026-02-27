using ManaChat.API.Clients;
using ManaChat.API.Helpers;
using ManaChat.API.Models.Auth;
using ManaChat.Core.Configuration;
using ManaChat.Core.Models.Auth;
using ManaChat.Identity.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;

namespace ManaChat.API.Middleware
{
    public class IdentityValidationMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context, IAuthenticatedUserDetails user, IUsersRepository userRepo, IOptions<ManaChatConfiguration> config)
        {
            var authedUser = user as AuthenticatedUserDetails ?? throw new Exception("Authenticated user details is not of type AuthenticatedUserDetails");
            var client = ClientFetcher.GetFromHeaders(context.Request.Headers);
            authedUser.UsesCookies = client.UsesCookies;

            if (IsEndpointAllowAnonymous(context))
            {
                authedUser.IsAuthenticated = false;
                await next(context);
                return;
            }

            var token = client.GetClientToken(context.Request);
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized", message = "No token provided" });
                return;
            }

            var tokenHash = TokenHelpers.HashToken(token);
            var session = await userRepo.GetUserSession(tokenHash);
            if (session.IsTorn)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", message = "Session is torn, please try again" });
                return;
            }

            if (session.IsFlowing && session.GetValue() == null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error", message = "Invalid Token" });
                return;
            }

            var sessionActual = session.GetValue()!;
            if (sessionActual.ExpiresAt < DateTimeOffset.UtcNow)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized", message = "Session expired" });
                return;
            }

            var settings = config.Value.TokenSettings;
            if (settings.Sliding && settings.CanRefreshCurrentToken(sessionActual.StartedAt, sessionActual.ExpiresAt))
            {
                await userRepo.UpdateUserSession(sessionActual.Id, sessionActual.UserId, tokenHash, DateTimeOffset.UtcNow.Add(settings.GetExpiryTimeSpan()));
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
