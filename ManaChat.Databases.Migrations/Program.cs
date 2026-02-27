using ManaFox.Databases.Migrations;
using Microsoft.Extensions.Configuration;
using Serilog;
using ManaChat.Core.Constants;
using ManaFox.Extensions.Flow;

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

        var loaderIdentity = typeof(ManaChat.Databases.Identity.AssemblyLoader).Assembly;
        var loaderMessaging = typeof(ManaChat.Databases.Messaging.AssemblyLoader).Assembly;

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

            var root = AppContext.BaseDirectory;

            if (doIdentity)
            {
                Log.Information("Starting identity database script generation...");
                var identityConnectionString = connectionStrings[DatabaseConstants.IdentityDatabaseKey] ?? throw new InvalidOperationException("Identity connection string not configured");

                var result = RuneMigrator.Create()
                    .Bind(m => m.WithConnectionString(identityConnectionString))
                    .Bind(m => m.WithSqlProject(Path.Combine(root, "../../../../ManaChat.Databases.Identity/ManaChat.Databases.Identity.sqlproj")))
                    //.Bind(m => m.GenerateTo(".Scripts/Identity"))
                    .Bind(m => m.CreateDatabaseIfNotExists())
                    .Bind(m => m.Deploy());

                if (!result.IsFlowing)
                {
                    Log.Error("Identity migration failed: {Error}", result.GetTear());
                    if (failOnError)
                        return 1;
                }
            }

            if (doMessaging)
            {
                Log.Information("Starting messaging database script generation...");
                var messagingConnectionString = connectionStrings[DatabaseConstants.MessagingDatabaseKey] ?? throw new InvalidOperationException("Messaging connection string not configured");

                var result = RuneMigrator.Create()
                    .Bind(m => m.WithConnectionString(messagingConnectionString))
                    .Bind(m => m.WithSqlProject(Path.Combine(root, "../../../../ManaChat.Databases.Messaging/ManaChat.Databases.Messaging.sqlproj")))
                    //.Bind(m => m.GenerateTo(".Scripts/Messaging"))
                    .Bind(m => m.CreateDatabaseIfNotExists())
                    .Bind(m => m.Deploy());

                if (!result.IsFlowing)
                {
                    Log.Error("Messaging migration failed: {Error}", result.GetTear());
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