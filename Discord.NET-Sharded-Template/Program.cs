using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Discord.Addons.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.Commands;
using Discord.NET_Sharded_Template.Services;

namespace Discord.NET_Sharded_Template
{
    class Program
    {
        static async Task Main()
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();

                    // Set the minimum log level that will be displayed in the console.
                    x.SetMinimumLevel(LogLevel.Information);
                })
                .ConfigureDiscordShardedHost((context, configuration) =>
                {
                    configuration.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose, // Define when logs should be sent to the console (depending on log severity).
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200,
                        TotalShards = 5 // Define the amount of shars to use, uses recommended amount when left empty.
                    };

                    configuration.Token = context.Configuration["Token"]; // Pass the token through the appsettings.json file.
                })
                .UseCommandService((context, config) =>
                {
                    config.CaseSensitiveCommands = false; // Define whether or not commands should be case-sensitive.
                    config.LogLevel = LogSeverity.Verbose; // Define when logs of the command service should be sent to the console (depending on log severity).
                    config.DefaultRunMode = RunMode.Sync; // Define the default run mode (whether or not to separate commands from the main gateway).
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<CommandHandler>();
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
