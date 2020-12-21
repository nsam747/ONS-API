using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acre.Backend.Ons.Data;
using Acre.Backend.Ons.Models;
using Acre.Backend.Ons.Services.Parsers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Acre.Backend.Ons
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var shouldSeedDatabase = args.Any(arg => string.Equals(arg, "seed=true", StringComparison.OrdinalIgnoreCase));
            if(shouldSeedDatabase) {
                using(var scope = host.Services.CreateScope()) {
                    var dbSeeder = scope.ServiceProvider.GetRequiredService<IOnsDbSeeder>();
                    await dbSeeder.SeedDatabase();
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(config);
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
