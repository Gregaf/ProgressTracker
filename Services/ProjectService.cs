using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ProgressTracker.Services
{
    public class ProjectService : IProjectService
    {
        public IConfiguration mConfig;

        private readonly ILogger<ProjectService> mLogger;

        private static HttpClient mClient;

        private string authToken;

        public string workingOrganization { get; private set; }

        private Project currentProject;
        private Dictionary<string, Project> projectLists;

        private class Project
        {
            public string name { get; }
            public string description { get; }
            public int id { get; }
            public Dictionary<string, Column> columnKvps { get; }

            public Project(string name, string description, int id)
            {
                this.name = name;
                this.description = description;
                this.id = id;
                columnKvps = new Dictionary<string, Column>();
            }
        }

        private class Column
        { 
            public string name { get; }
            public int id { get; }
            Dictionary<int, string> cardKvps { get; }

            public Column(string name, int id)
            {
                this.name = name;
                this.id = id;

                this.cardKvps = new Dictionary<int, string>();
            }

        }

        public ProjectService(IConfiguration configuration, ILogger<ProjectService> logger)
        {
            mConfig = configuration;
            mLogger = logger;

            projectLists = new Dictionary<string, Project>();

            mClient = new HttpClient();

            workingOrganization = "ProgTrackOrg";
            authToken = mConfig["GToken"];

            mClient.DefaultRequestHeaders.Clear();

            mClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Progress-Tracker-Bot", "1.0"));
            mClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mConfig["MediaType"]));
            mClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", authToken);
        }

        public async Task<bool> AddCard(string columnName, string body)
        {
            Uri myUri = CreateUri($"projects/columns/{currentProject.columnKvps[columnName].id}/cards");

            mLogger.LogInformation(myUri.ToString());

            JObject cardObject = new JObject();
            cardObject.Add("note", body);

            StringContent stringContent = new StringContent(cardObject.ToString());

            HttpResponseMessage response = await mClient.PostAsync(myUri, stringContent);

            response.EnsureSuccessStatusCode();

            return true;
        }

        public async Task<bool> AddColumn(string name)
        {
            Uri myUri = CreateUri($"projects/{currentProject.id}/columns");

            JObject jObject = new JObject();
            jObject.Add("name", name);

            StringContent stringContent = new StringContent(jObject.ToString());

            HttpResponseMessage postResponse = await mClient.PostAsync(myUri, stringContent);

            postResponse.EnsureSuccessStatusCode();

            String information = await postResponse.Content.ReadAsStringAsync();
            JObject responeInfo = JObject.Parse(information);

            Column newColumn = new Column(responeInfo["name"].ToString(), responeInfo["id"].ToObject<int>());

            currentProject.columnKvps.Add(name, newColumn);
            // Need to use the response content to get the columnID and then add it to the KVP of the current project.

            return true;
        }

        public async Task<bool> CreateProject(string name, string description = "")
        {
            Uri myUri = CreateUri($"orgs/{workingOrganization}/projects");

            mLogger.LogInformation($"Request sent to {myUri.ToString()}");

            JArray projectList = await GetProjectList();
              
            for (int i = 0; i < projectList.Count; i++)                
            {
                    
                if (projectList[i]["name"].ToString() == name)                        
                    return false;
         
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

        public async Task<bool> RemoveColumn(string headerName)
        {
            Uri myUri = CreateUri($"projects/columns/{currentProject.columnKvps[headerName].id}");

            HttpResponseMessage response = await mClient.DeleteAsync(myUri);

            response.EnsureSuccessStatusCode();

            currentProject.columnKvps.Remove(headerName);

            return true;
        }

        public async Task<bool> RemoveProject()
        {
            Uri myUri = CreateUri($"projects/{currentProject.id}");

            HttpResponseMessage response = await mClient.DeleteAsync(myUri);

            response.EnsureSuccessStatusCode();

            return true;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private async Task<JArray> GetProjectList()
        {
            Uri myUri = CreateUri($"orgs/{workingOrganization}/projects");

            HttpResponseMessage response = await mClient.GetAsync(myUri);

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                JArray projectList = JArray.Parse(content);

                mLogger.LogInformation($"{projectList.ToString()}");

                return projectList;
            }
            else
            {
                mLogger.LogWarning($"Was not a success status code, {response.StatusCode} was returned.");

                return null;
            }    
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

        public Task SetWorkingOrg(string orgName)
        {
            workingOrganization = orgName;

            return Task.CompletedTask;
        }

        public async Task SetWorkingProject(string projectName)
        {
            if (projectLists.ContainsKey(projectName))
            {
                currentProject = projectLists[projectName];
                return;
            }

            JArray projectList = await GetProjectList();

            for (int i = 0; i < projectList.Count; i++)
            {
                if (projectList[i]["name"].ToString() == projectName)
                {                   
                    Project newProject = new Project(projectName, projectList[i]["body"].ToString(), projectList[i]["id"].ToObject<int>());
                   
                    projectLists.Add(projectName, newProject);

                    currentProject = newProject;

                    break;
                }
            }

            JArray columnList = await GetProjectColumns();

            // Failed to get project columns.
            if (columnList == null)
            {
                return;
            }


            // Adds the kvp of column names associated with the id's that way can delete headers and such via the header names.
            for (int i = 0; i < columnList.Count; i++)
            {
                if (!currentProject.columnKvps.ContainsKey(columnList[i]["name"].ToString()))
                {
                    string columnName = columnList[i]["name"].ToString();
                    int columnID = columnList[i]["id"].ToObject<int>();

                    Column newColumn = new Column(columnName, columnID);



                    currentProject.columnKvps.Add(columnName, newColumn);
                }
            }

        }

        private async Task<JArray> GetProjectCards()
        {
            throw new NotImplementedException();
        }

        private async Task<JArray> GetProjectColumns()
        {
            Uri myUri = CreateUri($"projects/{currentProject.id}/columns");

            HttpResponseMessage response = await mClient.GetAsync(myUri);

            if (response.IsSuccessStatusCode)
            {
                string stringContent = await response.Content.ReadAsStringAsync();

                JArray columnList = JArray.Parse(stringContent);

                return columnList;
            }
            else
            {
                mLogger.LogWarning($"Status code came back as {response.StatusCode}!");

                return null;
            }

        }

        public string DisplayCurrentProject()
        {
            string info = $"`Name:{currentProject.name} | Body:{currentProject.description} | ID:{currentProject.id}`";

            return info;
        }

        public async Task<bool> DeleteProject()
        {
            Uri myUri = CreateUri($"projects/{currentProject.id}");

            HttpResponseMessage response = await mClient.DeleteAsync(myUri);

            response.EnsureSuccessStatusCode();

            return true;
        }

        public async Task<string[]> GetProjectNames()
        {
            JArray projectList = await GetProjectList();
            string[] namesList = new string[projectList.Count];

            for (int i = 0; i < projectList.Count; i++)
            {
                namesList[i] = projectList[i]["name"].ToString();
            }

            return namesList;
        }

    }
}
