using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectClasses;
using GetAnAzureIoTQuickstartApp.Shared;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GetAnAzureIoTQuickstartApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplesController : ControllerBase
    {
        // GET: api/<SamplesController>
        //[HttpGet]
        //public IEnumerable<IGrouping<char, KeyValuePair<string, List<Project>>>> Get()
        //{
        //    var alpha = WeatherForecast.AlphaSort;
        //    return alpha;
        //}

        static int count = 1;
        [HttpGet]
        public string Get()
        {
            var projIds = (from p in SamplesCollections.Projects select p.Id).ToList();
            var treeIds = (from t in SamplesCollections.Folders select t.Projects).ToList();
            var ll = treeIds.SelectMany(d => d).ToList();

            int countp = projIds.Count;
            int countt = ll.Count;
            var sdf = ll.Except(projIds).ToList();
            var sdf2 = projIds.Except(ll).ToList();

            string json = "";
            if (count == 0)
            {
                var projects = SamplesCollections.Projects;//.AlphaSort;
                json = JsonConvert.SerializeObject(projects);
                count = 1;
            }
            else
            {
                var folders = SamplesCollections.Folders;
                json = JsonConvert.SerializeObject(folders);
                count = 0;
            }
            return json;
        }

        // GET api/<SamplesController>/5
        [HttpGet("{param}")]
        public string Get(string param)
        {
            string[] names = param.Split(new char[] { '-' });
            string DeviceName = names[0];
            string ProjectName = names[1];
            string FileType = names[2];
            List<Project> projects = new List<Project>(); //
                //GetAnAzureIoTQuickstartApp.Shared.SamplesCollections.Projects[DeviceName];
            var xproject = from p in projects where p.Name == ProjectName select p;
            var project = xproject.FirstOrDefault();
            string path="";
            string text = "";
            if (FileType == "Image")
            {
                path = $"{project.Path}/{project.ProjectPNGFileName}";
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Bitmap qrCodeImage = new Bitmap(path))
                    {
                        qrCodeImage.Save(ms, ImageFormat.Png);
                        text = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            else
            {
                if (FileType == "SourceFile")
                    path = $"{project.Path}/{project.ProjectCSFileName}";
                else if (FileType == "ProjectFile")
                    path = $"{project.Path}/{project.ProjectFileName}";
                else if (FileType == "ProjectFileUse")
                    path = $"Pages//Project.csproj.txt";
                else if (FileType == "ReadMe")
                {
                    path = Path.GetFullPath(Path.Combine(project.Path, @"..\ReadMe.md"));
                }
                else if (FileType == "HowTo")
                {
                    path = $"Pages//HowTo.md"; ;
                }
                else if (FileType == "HowTo2")
                {
                    path = $"Pages//HowTo2.md"; ;
                }
                text = System.IO.File.ReadAllText(path);
            }
            return text;
        }

        /*
        // GET api/<SamplesController>/5
        [HttpGet("{ id} ")]
        public string Get(int id)
                    {
                        return "value";
                    }

                    // POST api/<SamplesController>
                    [HttpPost]
                    public void Post([FromBody] string value)
                    {
                    }

                    // PUT api/<SamplesController>/5
                    [HttpPut("{id}")]
                    public void Put(int id, [FromBody] string value)
                    {
                    }

                    // DELETE api/<SamplesController>/5
                    [HttpDelete("{id}")]
                    public void Delete(int id)
                    {
                    } */
    }
            
}
