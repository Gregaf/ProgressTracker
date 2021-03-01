using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ProgressTracker.Services
{
    public interface IProjectService : IHostedService
    {
        Task<string[]> GetProjectNames();
        string DisplayCurrentProject();
        Task SetWorkingOrg(string orgName);
        Task SetWorkingProject(string projectName);
        Task<JObject> GetProjectInformation();
        Task<bool> CreateProject(string name, string description);
        Task<bool> DeleteProject();
        Task<bool> AddColumn(string name);
        Task<bool> AddCard(string columnName, string body);
        Task<bool> RemoveProject();
        Task<bool> RemoveColumn(string headerName);
        Task<bool> RemoveCard();
    }
}
