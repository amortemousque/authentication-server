using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace AuthorizationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "AuthorizationServer";

            //var seed = args.Any(x => x == "/seed");
            //if (seed) args = args.Except(new[] { "/seed" }).ToArray();

            var host = BuildWebHost(args);

          
            //SeedData.EnsureSeedData(host.Services);
            SeedData.EnsureSeedDataServer4(host.Services);

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(@"identityserver4_log.txt")
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .CreateLogger();


            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("hosting.json", optional: true)
               .AddEnvironmentVariables()
               .AddCommandLine(args)
               .Build();

            return WebHost.CreateDefaultBuilder(args)
				.UseConfiguration(config)
				.ConfigureLogging(builder =>
				{
					builder.ClearProviders();
					builder.AddSerilog();
				})
				.UseStartup<Startup>()
                .Build();
        }
    }
}
