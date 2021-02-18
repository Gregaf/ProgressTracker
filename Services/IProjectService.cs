using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ProgressTracker.Services
{
    public interface IProjectService : IHostedService
    {
        Task<JObject> GetProjectInformation();
        Task<bool> CreateProject(string path, string name, string description);
        Task<bool> AddColumn();
        Task<bool> AddCard();
        Task<bool> RemoveProject();
        Task<bool> RemoveColumn();
        Task<bool> RemoveCard();
    }
}
