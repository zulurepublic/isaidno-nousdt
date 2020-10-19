using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OKexTime.Context;
using Serilog;
using Serilog.Events;

namespace OKexTime
{
    public class Program
    {
        private static IConfigurationRoot Configuration;
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Timestamp:dd:MM:yyyy-HH:mm:ss} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            try
            {
                Log.Information("Building web host");
                var webHost = CreateHostBuilder(args).Build();
                Log.Information("Applying migrations");
                using (var context = (OkexContext)webHost.Services.GetService(typeof(OkexContext)))
                {
                    context.Database.Migrate();
                }

                Log.Information("Migrations Applied");

                Log.Information("Starting web host");
                webHost.Run();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 5000,
                            listenOptions => { listenOptions.Protocols = HttpProtocols.Http1; });
                    });

                    webBuilder.UseStartup<Startup>();
                });

    }
}
