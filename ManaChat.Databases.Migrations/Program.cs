using ManaChat.Core.Constants;
using ManaFox.Databases.Migrations;
using ManaFox.Extensions.Flow;
using Microsoft.Build.Evaluation;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text;

internal class Program
{
    private static int Main(string[] args)
    {
        bool doMessaging = true;
        bool doIdentity = true;
        bool failOnError = false;
        if (args.Length > 0)
        {
            doMessaging = args.Contains("--messaging", StringComparer.OrdinalIgnoreCase);
            doIdentity = args.Contains("--identity", StringComparer.OrdinalIgnoreCase);
            failOnError = args.Contains("--stop-on-error", StringComparer.OrdinalIgnoreCase);
        }

#pragma warning disable IDE0059 // Unnecessary assignment of a value
        var loaderIdentity = typeof(ManaChat.Databases.Identity.AssemblyLoader).Assembly;
        var loaderMessaging = typeof(ManaChat.Databases.Messaging.AssemblyLoader).Assembly;
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
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
                var res = MigrateDb(DatabaseConstants.IdentityDatabaseKey, identityConnectionString, "../../../../ManaChat.Databases.Identity/ManaChat.Databases.Identity.sqlproj", failOnError);
                if (res > 0)
                    return 1;
            }

            if (doMessaging)
            {
                var messagingConnectionString = connectionStrings[DatabaseConstants.MessagingDatabaseKey] ?? throw new InvalidOperationException("Messaging connection string not configured");
                var res = MigrateDb(DatabaseConstants.MessagingDatabaseKey, messagingConnectionString, "../../../../ManaChat.Databases.Messaging/ManaChat.Databases.Messaging.sqlproj", failOnError);
                if (res > 0)
                    return 1;
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

    private static int MigrateDb(string key, string connString, string sqlPath, bool failOnError)
    {
        Log.Information($"Starting {key} database migration...");
        var root = AppContext.BaseDirectory;

        var result = RuneMigrator.Create()
            .Bind(m => m.WithConnectionString(connString))
            .Scry(m => Log.Verbose("Connection string bound"))
            .Bind(m => m.WithSqlProject(Path.Combine(root, sqlPath)))
            .Scry(m => Log.Verbose($"Sql project connected"))
            .Bind(m => m.CreateDatabaseIfNotExists())
            .Scry(m => Log.Verbose($"Db exists"))
            .Bind(m => m.Deploy());

        if (!result.IsFlowing)
        {
            Log.Error("Identity migration failed: {Error}", result.GetTear());
            if (failOnError)
                return 1;
        }
        else
        {
            var val = result.GetValue()!;
            Log.Information($"{val.DatabaseName} migration complete. Took: {val.Duration} to deploy {val.DacpacsDeployed} DACPACs.");
            Log.Debug(val.GetDeploymentResultsTable());        
        }
        Log.Information($"Finished {key} database migration...");
        return 0;
    }
}