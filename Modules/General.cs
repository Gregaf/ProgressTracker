using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace ProgressTracker.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<General> _logger;
        public General(ILogger<General> logger) => _logger = logger;   
     

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
            _logger.LogInformation($"{Context.User.Username} executed the ping command!");
        }

        [Command("info")]
        public async Task Info()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.WithThumbnailUrl(Context.User.GetDefaultAvatarUrl());
            builder.WithDescription("ProgressTracker is is a bot designed to allow the creation of project boards within Discord!");
            builder.WithColor(new Color(66, 245, 173));
            builder.WithCurrentTimestamp();

            Embed embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("server")]
        public async Task Server()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithThumbnailUrl(Context.Guild.IconUrl);
            builder.WithDescription("Below will be some information about this server!\n");
            builder.WithTitle($"{Context.Guild.Name} Information");
            builder.WithColor(Color.Blue);
            builder.AddField("Created at", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true);
            builder.AddField("MemeberCount", (Context.Guild as SocketGuild).MemberCount + " members", true);
            builder.AddField("Online Users", (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Online).Count() + " memebers", true);

            Embed embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}
