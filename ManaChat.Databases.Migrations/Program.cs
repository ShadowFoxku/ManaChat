using ManaFox.Databases.Migrations;
using Microsoft.Extensions.Configuration;
using Serilog;
using ManaFox.Extensions.Flow;
using ManaChat.Core.Constants;

internal class Program
{
    private static int Main(string[] args)
    {
        bool doMessaging = true;
        bool doIdentity = true;
        bool failOnError = false;
        if (args.Length > 0)
        {
            doMessaging = doIdentity = false; // If any arguments are provided, default to not running any migrations unless specified
            doMessaging = args.Contains("--messaging", StringComparer.OrdinalIgnoreCase);
            doIdentity = args.Contains("--identity", StringComparer.OrdinalIgnoreCase);
            failOnError = args.Contains("--stop-on-error", StringComparer.OrdinalIgnoreCase);
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();

            var connectionStrings = config.GetSection("ConnectionStrings");

            if (doIdentity)
            {
                var identityConnectionString = connectionStrings[DatabaseConstants.IdentityDatabaseKey] ?? throw new InvalidOperationException("Identity connection string not configured");
                Log.Information("Starting identity database migrations...");
                var identityResult = RuneMigrator
                    .Create(identityConnectionString)
                    .Bind(m => m.WithEmbeddedScripts(typeof(ManaChat.Databases.Identity.AssemblyLoader).Assembly))
                    .Bind(m => m.UseDefaultFolderPattern())
                    .Bind(m => m.Run());

                if (!identityResult.IsFlowing)
                {
                    Log.Error("Identity database migration failed: {Error}", identityResult.GetTear());
                    if (failOnError)
                        return 1; 
                }
            }

            if (doMessaging)
            {
                var messagingConnectionString = connectionStrings[DatabaseConstants.MessagingDatabaseKey] ?? throw new InvalidOperationException("Messaging connection string not configured");
                Log.Information("Starting messaging database migrations...");
                var messagingResult = RuneMigrator
                    .Create(messagingConnectionString)
                    .Bind(m => m.WithEmbeddedScripts(typeof(ManaChat.Databases.Messaging.AssemblyLoader).Assembly))
                    .Bind(m => m.UseDefaultFolderPattern())
                    .Bind(m => m.Run());

                if (!messagingResult.IsFlowing)
                {
                    Log.Error("Messaging database migration failed: {Error}", messagingResult.GetTear()); 
                    if (failOnError)
                        return 1;
                }
            }
            Log.Information("All migrations completed");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Migration execution failed");
            return 1;
        }
    }
}