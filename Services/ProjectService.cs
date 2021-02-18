using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ProgressTracker.Services
{
    public class ProjectService : IProjectService
    {
        public IConfiguration _config;

        private static HttpClient mClient;

        private string authToken;
        private string workingProject;
        


        public ProjectService(IConfiguration configuration)
        {
            _config = configuration;
            mClient = new HttpClient();

            authToken = _config["GToken"];

            mClient.DefaultRequestHeaders.Clear();

            mClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Progress-Tracker-Bot", "1.0"));
            mClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_config["MediaType"]));
            mClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", authToken);
        }

        public Task<bool> AddCard()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddColumn()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateProject(string path, string name, string description = "")
        {
            Uri myUri = CreateUri(path);

            Console.WriteLine(myUri);

            HttpResponseMessage getResponse = await mClient.GetAsync(myUri);

            if (getResponse.IsSuccessStatusCode)
            {
                string content = await getResponse.Content.ReadAsStringAsync();
                JArray projectList = JArray.Parse(content);

                for (int i = 0; i < projectList.Count; i++)
                {
                    if (projectList[i]["name"].ToString() == name)                
                        return false;
                    
                }
            }

            JObject jObject = new JObject();

            jObject.Add("name", name);
            jObject.Add("body", description);

            StringContent stringContent = new StringContent(jObject.ToString());
            
            HttpResponseMessage postResponse = await mClient.PostAsync(myUri, stringContent);

            postResponse.EnsureSuccessStatusCode();

            return true;
        }

        public Task<JObject> GetProjectInformation()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveCard()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveColumn()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveProject()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        
        private Uri CreateUri(string path, params string[] querys)
        {
            UriBuilder uriBuilder = new UriBuilder();

            uriBuilder.Scheme = "https";
            uriBuilder.Host = "api.github.com";
            uriBuilder.Path = path;

            if (querys != null)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < querys.Length; i++)
                {
                    sb.Append(querys[i]);

                    if (i + 1 < querys.Length)
                        sb.Append("&");
                }
                uriBuilder.Query = sb.ToString();
            }

            return (uriBuilder.Uri);
        }
    }
}
