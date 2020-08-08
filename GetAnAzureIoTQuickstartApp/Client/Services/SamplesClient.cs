using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using ProjectClasses;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.JSInterop;

namespace GetAnAzureIoTQuickstartApp.Client.Services
{
    public class AlphaCount
    {
        public char Key { get; set; }
        public int Count { get; set; } = 0;
    }

    public class SampleCount
    {
        public string Key { get; set; }
        public int Count { get; set; } = 0;
    }
    public class SamplesClient
    {
        private readonly HttpClient client;
        private const string ServiceEndpoint = "/api/Samples";

        public Project[] Projects { get; set; }
        public FolderTree[] Folders { get; set; }
        public FolderTree RootFolder { get; set; }
        public static Dictionary<int,FolderTree>FolderDict {get; set;}
        public static Dictionary<int, Project> ProjectDict { get; set; }

        public static char CurrentIndex { get; set; } = '0';


        public static Project CurrentProject { get; set; } = null;

        public SamplesClient(HttpClient client)
            {
                this.client = client;
            }

        public async Task<string> GetFile (string path)
        {
            string fileContents = "";
            var strn = await client.GetAsync(ServiceEndpoint + $"/{path}");
            fileContents = await strn.Content.ReadAsStringAsync();
            return fileContents;
        }

        public async Task< FolderTree[]> Get()
        {
            var strn = await client.GetAsync(ServiceEndpoint);
            string content = await strn.Content.ReadAsStringAsync();
            Folders =  JsonConvert.DeserializeObject<FolderTree[]>(content);
            RootFolder = Folders[0];
            FolderDict = new Dictionary<int, FolderTree>();
            foreach (var folder in Folders)
            {
                FolderDict.Add(folder.Id, folder);
                var asd = FolderDict[folder.Id];
            }

            var strn1 = await client.GetAsync(ServiceEndpoint);
            string content1 = await strn1.Content.ReadAsStringAsync();
            Projects = JsonConvert.DeserializeObject<Project[]>(content1);

            ProjectDict = new Dictionary<int, Project>();
            foreach (Project project in Projects)
            {
                ProjectDict.Add(project.Id, project);
            }
            var keys = ProjectDict.Keys.ToList();

            var projIds = (from p in Projects select p.Id).ToList();
            var treeIds = (from t in Folders select t.Projects).ToList();
            var ll = treeIds.SelectMany(d => d).ToList();

            var sdf = ll.Except(projIds).ToList();
            var sdf2 = projIds.Except(ll).ToList();


            foreach (Project project in Projects)
            {
                if (!keys.Contains(project.Id))
                {
                    System.Diagnostics.Debug.WriteLine(project.Id);
                }
            }
            return Folders;    
        }
            

    }
}
