using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ViewBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //.UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostContext, config) => AddConfigFiles(config))
                .UseStartup<Startup>()
                .UseUrls("http://*:5000")
                .ConfigureLogging((context, logging) =>
                {//Suppress ConsoleLogger https://stackoverflow.com/questions/45986517/remove-console-and-debug-loggers-in-asp-net-core-2-0-when-in-production-mode
                    // clear all previously registered providers
                    logging.ClearProviders();
                    // now register everything you *really* want
                    logging.AddDebug();
                })
                .Build();

        private static void AddConfigFiles(IConfigurationBuilder config)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/#add-configuration-providers
            //config.SetBasePath(Directory.GetCurrentDirectory());
            config.AddJsonFile("hosting.json", optional: true);

        }
    }
}