using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace ProgressTracker.Modules
{
    // This will be the data obtained from the Github API


    public class Data
    {
        // Column -> ID, Header, Card[]
        // Card -> ID, Body

    }

    public class ProjectBoard : ModuleBase<SocketCommandContext>
    {
        static HttpClient client = new HttpClient();
        private readonly ILogger<General> _logger;
        public ProjectBoard(ILogger<General> logger) => _logger = logger;   
        
    }
}
