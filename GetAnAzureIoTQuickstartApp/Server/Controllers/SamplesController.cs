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
using System.IO.Compression;

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
            string text = "404";
            System.Diagnostics.Debug.WriteLine(param);
            string[] names = param.Split(new char[] { '~' });
            string FileName = names[0];
            string FolderId = names[1];
            int foldId;
            if (int.TryParse(FolderId, out foldId))
            {
                var fld = from f in FolderTree.AllFolderTrees where f.Id == foldId select f;
                var folder = fld.First();
                string fpath = folder.FolderPath;
                if (FileName.Contains(".zip"))
                {
                    string zipPath = $".\\Downloads\\{FileName}";
                    if (!System.IO.File.Exists(zipPath))
                    {
                        ZipFile.CreateFromDirectory(fpath, zipPath);
                        text = "Zipped";
                    }
                    else
                        text = "Already zipped";
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (FileStream file = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] bytes = new byte[file.Length];
                            int len1 = bytes.Length;
                            file.Read(bytes, 0, (int)file.Length);
                            ms.Write(bytes, 0, (int)file.Length);
                        }
                        
                        //text = "data:application/octet-stream;base64," + Convert.ToBase64String(ms.ToArray(), "application/zip");
                        text = Convert.ToBase64String(ms.ToArray());
                        int len2 = text.Length;
                            //"application/zip"); ; ; ; // ;// ;

                    }
                }
                else
                {
                    string path = Path.Combine(fpath, FileName);
                    path = path.Replace("\\\\", "\\");
                    text = System.IO.File.ReadAllText(path);
                }
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
