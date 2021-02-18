using Discord.Commands;
using Microsoft.Extensions.Logging;
using ProgressTracker.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProgressTracker.Modules
{
    public class GithubProject : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<General> _logger;
        private readonly IProjectService projectService;

        public GithubProject(ILogger<General> logger, IProjectService projectService)
        {
            _logger = logger;
            this.projectService = projectService;
        }

        //display -- Will get the default projectboard.
        [Command("create")]
        public async Task Create()
        {
            bool success = await projectService.CreateProject("orgs/ProgTrackOrg/projects", "Hello-Bot", "Looks like I got something!");

            if (!success)
            {
                _logger.LogInformation("Failed to create project.");
                return;
            }

            await ReplyAsync("I managed to create that project!");
        }


    }
}
