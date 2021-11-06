using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace School.Member.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            try
            {
                Console.WriteLine("Starting application...");
                var host = CreateHostBuilder(args).Build();
                Migrate(host.Services);
                host.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Application crashed.");
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("Application shut down.");
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
