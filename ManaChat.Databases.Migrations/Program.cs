using ManaChat.Core.Constants;
using ManaChat.Core.Helpers;
using ManaFox.Core.ConsoleTools;
using ManaFox.Databases.Migrations;
using ManaFox.Extensions.Flow;
using Microsoft.Extensions.Configuration;

namespace ManaChat.Databases.Migrations;

public class Program
{
    private static int Main(string[] args)
    {
        bool doMessaging = true;
        bool doIdentity = true;
        if (args.Length > 0)
        {
            doMessaging = args.Contains("--messaging", StringComparer.OrdinalIgnoreCase);
            doIdentity = args.Contains("--identity", StringComparer.OrdinalIgnoreCase);
        }

        if (!doMessaging && !doIdentity)
        {
            Console.WriteLine("Both databases have been marked to skip - nothing to do");
            return 0;
        }

        try
        {
            DatabaseMigrator migrator = new DatabaseMigrator();
            bool isSuccess = true;

            if (doIdentity)
                isSuccess &= migrator.MigrateIdentity();

            if (doMessaging)
                isSuccess &= migrator.MigrateMessaging();

            if (isSuccess)
            {
                Console.WriteLine($"{ConsoleConstants.BrightGreen}[✓] All migrations completed!{ConsoleConstants.Reset}");
                return 0;
            }
            
            Console.WriteLine($"{ConsoleConstants.BrightGreen}[x] One or more migrations failed.{ConsoleConstants.Reset}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration execution failed; {ex.Message}");
            return 1;
        }
    }
}