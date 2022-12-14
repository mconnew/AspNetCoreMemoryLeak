using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BasicAspNetCoreHelloWorld
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            while (true)
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break(); // Capture heap snapshot
                }
                await RunStressAsync(100);
                await Task.Delay(TimeSpan.FromMinutes(5));
                GC.Collect(2);
                GC.Collect(2);
            }
        }

        private static async Task RunStressAsync(int runCount)
        {
            for (int i = 0; i < runCount; i++)
            {
                await RunStressRound2();
            }
        }

        private static async Task RunStressRound1()
        {
            var builder = WebApplication.CreateBuilder(Array.Empty<string>());
            builder.WebHost.UseKestrel(options =>
            {
                options.ListenLocalhost(8443);
            });
            builder.Logging.ClearProviders();
            await using (var app = builder.Build())
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Hello world!");
                });
                await app.StartAsync();
                await app.StopAsync();
            }
        }

        private static async Task RunStressRound2()
        {
            using (var host = WebHost.CreateDefaultBuilder()
                .UseKestrel(options =>
                {
                    options.ListenLocalhost(8443);
                })
                .ConfigureLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                })
                .UseStartup<Startup>()
                .Build())
            {
                await host.StartAsync();
                await host.StopAsync();
            }
        }

        private class Startup
        {
            public void Configure(IApplicationBuilder app)
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Hello world!");
                });
            }
        }
    }
}