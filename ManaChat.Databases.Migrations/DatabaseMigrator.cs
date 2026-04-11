using ManaChat.Core.Constants;
using ManaChat.Core.Helpers;
using ManaFox.Core.ConsoleTools;
using ManaFox.Databases.TSQL.Migrations;
using ManaFox.Extensions.Flow;
using Microsoft.Extensions.Configuration;

namespace ManaChat.Databases.Migrations;

public class DatabaseMigrator
{
    private readonly IConfigurationSection _connectionStrings;
    
    public DatabaseMigrator()
    {
        ManaConsole.Init();
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .Build();
        
        _connectionStrings = config.GetSection("ConnectionStrings");
    }

    public bool MigrateIdentity() => DoMigration(DatabaseConstants.IdentityDatabaseKey);

    public bool MigrateMessaging() => DoMigration(DatabaseConstants.MessagingDatabaseKey);

    private bool DoMigration(string key)
    {
        var connString = _connectionStrings[key] ?? throw new InvalidOperationException($"{key} connection string not configured");
        
        ManaLoader.ShowMigrating(key);
        var sqlPath = $"ManaChat.Databases.{key}.dacpac";

        var result = RuneMigrator.Create()
            .Bind(m => m.WithConnectionString(connString))
            .Bind(m => m.WithDacpac(Path.Combine(AppContext.BaseDirectory, sqlPath)))
            .Bind(m => m.CreateDatabaseIfNotExists())
            .Bind(m => m.Deploy());

        if (!result.IsFlowing)
        {
            Console.WriteLine($"{key} migration failed: {result.GetTear()}");
            ManaLoader.ShowMigrationFailed(key);
        }
        else
        {
            var val = result.GetValue()!;
            Console.WriteLine($"{val.DatabaseName} migration complete. Took: {val.Duration} to deploy {val.DacpacsDeployed} DACPACs.");
            Console.WriteLine(val.GetDeploymentResultsTable());
            ManaLoader.ShowMigrationComplete(key);
        }

        return result.IsFlowing;
    }
}