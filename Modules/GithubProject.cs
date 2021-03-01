using Discord;
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

        [Command("getNames")]
        public async Task GetNames()
        {
            string[] names = await projectService.GetProjectNames();

            EmbedBuilder builder = new EmbedBuilder();

            foreach (string name in names)
            {
                builder.AddField("Name: ", name, true);
            }

            Embed embed = builder.Build();

            await Context.Message.ReplyAsync("", false, embed);
            
        }

        //display -- Will get the default projectboard.
        [Command("create")]
        public async Task Create(string name, string description = "")
        {
            bool success = await projectService.CreateProject(name, description);

            if (!success)
            {
                _logger.LogWarning("Failed to create project, name already exists.");
                await ReplyAsync("Looks like that project name already exists!");
                return;
            }
            else
            {

                await ReplyAsync($"I managed to create {name}!");
            }
        }

        [Command("setProject")]
        public async Task Create(string name)
        {
            await projectService.SetWorkingProject(name);

            await ReplyAsync(projectService.DisplayCurrentProject());

        }

        [Command("addCard")]
        public async Task AddCard(string columnName, string body)
        {
            bool success = await projectService.AddCard(columnName, body);

            string responseMessage = success ? "Successful Add" : "Failed to add";

            await ReplyAsync(responseMessage);
        }

        [Command("addColumn")]
        public async Task AddColumn(string name)
        {
            bool success = await projectService.AddColumn(name);

            string responseMessage = success ? "Successful Add" : "Failed to add";

            await ReplyAsync(responseMessage);
        }

        [Command("deleteProject")]
        public async Task DeleteProject()
        {
            bool success = await projectService.DeleteProject();

            string responseMessage = success ? "Successful delete" : "Failed to delete";

            await ReplyAsync(responseMessage);
        }

        [Command("removeColumn")]
        public async Task RemoveColumn(string headerName)
        {
            bool success = await projectService.RemoveColumn(headerName);

            string responseMessage = success ? "Successful delete" : "Failed to delete";

            await ReplyAsync(responseMessage);
        }
    }
}
