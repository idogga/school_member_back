using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace School.Member.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(config)
                .WriteTo.Console()
                .WriteTo.Sentry(o =>
                    {
                        // Debug and higher are stored as breadcrumbs (default is Information)
                        o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                        // Warning and higher is sent as event (default is Error)
                        o.MinimumEventLevel = LogEventLevel.Warning;
                    })
                .CreateLogger();

            try
            {
                Log.Information("Starting application...");
                var host = CreateHostBuilder(args).Build();
                Migrate(host.Services);
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application crashed.");
            }
            finally
            {
                Log.Information("Application shut down.");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void Migrate(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<MemberDbContext>();
            var logger = scope.ServiceProvider.GetService<ILogger<Startup>>();

            if (context == null)
            {
                logger.LogError("Not found context");
                return;
            }

            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                var migrations = string.Join("\n", pendingMigrations);
                logger.LogInformation($"Pending migrations:{migrations}");
            }
            else
            {
                logger.LogInformation($"No pending migrations");
                return;
            }

            try
            {
                context.Database.Migrate();
                logger.LogInformation($"Migrate success!");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Error while migrate");
                throw;
            }
        }
    }
}
