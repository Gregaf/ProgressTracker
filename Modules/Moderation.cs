using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace ProgressTracker.Modules
{
    public class Moderation : ModuleBase
    {
        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount).FlattenAsync();

            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            int messageCount = (messages as ICollection<IMessage>).Count;

            IUserMessage message = await Context.Channel.SendMessageAsync($"**Successfully deleted {messageCount} messages!**");

            await Task.Delay(2500);

            await message.DeleteAsync();

            Console.WriteLine("Completed deleting all messages.");
        }

    }
}
