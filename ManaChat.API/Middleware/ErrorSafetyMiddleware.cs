using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Serialization;

namespace ManaChat.API.Middleware
{
    public class ErrorSafetyMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context, ILogger<ErrorSafetyMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred while processing the request.");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new { HttpStatusCode = context.Response.StatusCode.ToString(), Error = "An unexpected error occurred." }));
            }
        }
    }
}
