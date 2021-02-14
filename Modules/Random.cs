using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;


namespace ProgressTracker.Modules
{
    public class Random : ModuleBase<SocketCommandContext>
    {

        private readonly ILogger<Random> _logger;
        public Random(ILogger<Random> logger) => _logger = logger;

        [Command("lulz")]
        public async Task Meme(string subreddit = null)
        {
            HttpClient client = new HttpClient();
            string result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "ProgrammerHumor"}/random.json?limit=1");

            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("This subreddit doesnt exist!");
                return;
            }

            JArray jsonArray = JArray.Parse(result);
            Console.WriteLine(jsonArray.ToString());
            JObject post = JObject.Parse(jsonArray[0]["data"]["children"][0]["data"].ToString());

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithImageUrl(post["url"].ToString());
            builder.WithColor(Color.Red);
            builder.WithTitle(post["title"].ToString());
            builder.WithUrl("https://reddit.com" + post["permalink"].ToString());
            // builder.WithFooter("");
            Embed embed = builder.Build();

            await Context.Channel.SendMessageAsync(null, false, embed);
        }


    }
}
