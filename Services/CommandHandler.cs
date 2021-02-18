using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Discord.Addons.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Discord;

namespace ProgressTracker.Services
{
    public class CommandHandler : InitializedService
    {
        public IServiceProvider _provider;
        public static DiscordSocketClient _client;
        public static CommandService _service;
        public static IConfiguration _config;
        public static IProjectService projectService;
        
        public CommandHandler(DiscordSocketClient discord, CommandService commands,IConfiguration config, IServiceProvider provider)
        {
            _provider = provider;
            _client = discord;
            _service = commands;
            _config = config;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            _client.MessageReceived += OnMessageReceived;
            _client.ChannelCreated += OnChannelCreated;
            _service.CommandExecuted += OnCommandExecuted;
            _client.JoinedGuild += OnJoinedGuild;
        }

        private async Task OnJoinedGuild(SocketGuild arg)
        {
            await arg.DefaultChannel.SendMessageAsync("Hello there, ask !help to see what commands I provide!");
        }

        private async Task OnChannelCreated(SocketChannel arg)
        {
            if((arg as ITextChannel) == null) return; 

            ITextChannel channel = arg as ITextChannel;

            await channel.SendMessageAsync("This event was called!");
        }


        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) 
                return;
            if (message.Source != MessageSource.User) 
                return;

            int pos = 0;

            if (message.HasStringPrefix(_config["prefix"], ref pos) || message.HasMentionPrefix(_client.CurrentUser, ref pos))
            {
                SocketCommandContext context = new SocketCommandContext(_client, message);
                await _service.ExecuteAsync(context, pos, _provider);
            }


        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result}");    
        }
    }
}
