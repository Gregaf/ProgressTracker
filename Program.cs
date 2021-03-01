using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Addons.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using Discord.Commands;
using ProgressTracker.Services;
using System;

namespace ProgressTracker
{
    class Program
    {
        static async Task Main()
        {
            HostBuilder builder = new HostBuilder();
            builder.ConfigureAppConfiguration(x =>
            {
                ConfigurationBuilder configuration = new ConfigurationBuilder();
                configuration.SetBasePath($"{Directory.GetCurrentDirectory()}/Configurations").AddJsonFile("config.json", false, true);
                
                IConfigurationRoot root = configuration.Build();

                x.AddConfiguration(root);
            });

            builder.ConfigureLogging(x =>
            {
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Debug);
            });

            builder.ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
            {
                config.SocketConfig = new DiscordSocketConfig()
                {
                    LogLevel = LogSeverity.Verbose,
                    AlwaysDownloadUsers = true,
                    MessageCacheSize = 200
                };

                config.Token = context.Configuration["DToken"];
            });

            builder.UseCommandService((context, config) =>
            {
                config = new CommandServiceConfig()
                {
                    CaseSensitiveCommands = true,
                    LogLevel = LogSeverity.Verbose
                };

            });
            
            

            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(typeof(IProjectService), typeof(ProjectService));
                services.AddSingleton(typeof(IHostedService), typeof(CommandHandler));
                       
                        
            });

            builder.UseConsoleLifetime();

            IHost host = builder.Build();
                       
            using (host)
            {
                await host.RunAsync();
            }
        }


    }
}
