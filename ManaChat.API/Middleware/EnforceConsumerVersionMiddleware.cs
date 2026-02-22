using ManaChat.API.Clients;
using Newtonsoft.Json;

namespace ManaChat.API.Middleware
{
    public class EnforceConsumerVersionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task Invoke(HttpContext context, ILogger<EnforceConsumerVersionMiddleware> logger)
        {
            if (!context.Request.Headers.TryGetValue("X-Client-Version", out var version) || string.IsNullOrEmpty(version))
            {
                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug("Call to endpoint made, but no version header was provided.");
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("A version is required in X-Client-Version header");
                return;
            }

            var client = ClientFetcher.GetFromHeaders(context.Request.Headers);

            if (!RequestVersionIsValid(client, version.ToString()))
            {
                if (logger.IsEnabled(LogLevel.Information))
                    logger.LogInformation(
                        "Call to endpoint made, but the app version was {version}, which is below the minimum {MinimumConsumerMajorVersion}.{MinimumConsumerMinorVersion}.{MinimumConsumerPatchVersion}",
                        version,
                        client.MinimumSupportedVersion.Major,
                        client.MinimumSupportedVersion.Minor,
                        client.MinimumSupportedVersion.Patch);

                context.Response.StatusCode = StatusCodes.Status426UpgradeRequired;
                await context.Response.WriteAsync(JsonConvert.ToString(new ResultBody(client.MinimumSupportedVersion)));
                return;
            }

            await _next(context);
        }

        private static bool RequestVersionIsValid(ManaChatClient client, string userVersion)
        {
            var vParts = userVersion.Split('.').Select(int.Parse).ToList();

            if (vParts[0] < client.MinimumSupportedVersion.Major)
                return false;

            if (vParts[1] < client.MinimumSupportedVersion.Minor)
                return false;

            if (vParts[2] < client.MinimumSupportedVersion.Patch)
                return false;

            return true;
        }

        private class ResultBody(ManaChatVersion version)
        {
            public string Error = "Unsupported Client Version";
            public string Message = "Please upgrade your application to the latest version.";
            public string MinimumVersion { get; } = $"{version.Major}.{version.Minor}.{version.Patch}";
        }
    }
}
