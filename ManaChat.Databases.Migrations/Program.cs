using ManaChat.Core.Constants;
using ManaChat.Core.Helpers;
using ManaFox.Core.ConsoleTools;
using ManaFox.Databases.Migrations;
using ManaFox.Extensions.Flow;
using Microsoft.Extensions.Configuration;

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

        ManaConsole.Init();
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
            Console.WriteLine($"{ConsoleConstants.BrightGreen}[✓] All migrations completed!{ConsoleConstants.Reset}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration execution failed; {ex.Message}");
            return 1;
        }
    }

    private static int MigrateDb(string key, string connString, string sqlPath, bool failOnError)
    {
        ManaLoader.ShowMigrating(key);
        var root = AppContext.BaseDirectory;

        var result = RuneMigrator.Create()
            .Bind(m => m.WithConnectionString(connString))
            .Bind(m => m.WithSqlProject(Path.Combine(root, sqlPath)))
            .Bind(m => m.CreateDatabaseIfNotExists())
            .Bind(m => m.Deploy());

        if (!result.IsFlowing)
        {
            Console.WriteLine($"Identity migration failed: {result.GetTear()}");
            ManaLoader.ShowMigrationFailed(key);
            if (failOnError)
                return 1;
        }
        else
        {
            var val = result.GetValue()!;
            Console.WriteLine($"{val.DatabaseName} migration complete. Took: {val.Duration} to deploy {val.DacpacsDeployed} DACPACs.");
            Console.WriteLine(val.GetDeploymentResultsTable());
            ManaLoader.ShowMigrationComplete(key);
        }
        return 0;
    }
}