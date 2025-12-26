using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using System.Runtime;

namespace Migrations;
public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Contains("--dryrun"))
        {
            return;
        }
        // Получаем переменную среды, отвечающую за окружение
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                              throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT in not set");
        // собираем конфигурацию на основании окружения
        // у нас будет два варианта - Development/Production
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environmentName}.json")
            .Build();
        // Получаем строку подключения из конфига `appsettings.{Environment}.json`
        var connectionString = config["DbSettings:MigrationConnectionString"];
        var migrationRunner = new MigratorRunner(connectionString);
        // Мигрируемся
        migrationRunner.Migrate();
    }
}