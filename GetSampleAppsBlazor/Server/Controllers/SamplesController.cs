using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectClasses;
using GetSampleApps.Shared;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO.Compression;
using System.Runtime.InteropServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GetSampleApps.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplesController : ControllerBase
    {
        public static string DefaultPath { get; set; } = "";
        public static string ZipFolder { get; set; } = "";
        public static string GenerateTextPath { get; set; } = "";
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
            var projects = SamplesCollections.Projects;
            string json1 = JsonConvert.SerializeObject(projects);

            var folders = SamplesCollections.Folders;
            string json2 =  JsonConvert.SerializeObject(folders);
            json = json2 + "~" + json1;
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
            string FileType = "";
            if (names.Length > 2)
                FileType = names[2];
            FileType = FileType.ToUpper();
            int foldId;
            if (FileType=="CLEAR")
            {
                var files = Directory.GetFiles("Downloads");
                foreach (string f in files)
                {
                    System.IO.File.Delete(f);
                }
                var files2 = Directory.GetFiles("Uploads");
                foreach (string f in files2)
                {
                    System.IO.File.Delete(f);
                }
                text = "OK";
            }
            else if (FileType=="RELOAD")
            {
                var rootSample = GetSamples.GetSamplesProjects.GetFolders(DefaultPath, GenerateTextPath);
                GetSampleApps.Shared.SamplesCollections.Init(
                    rootSample
                );
                text = "OK";
            }
            else {
                if (int.TryParse(FolderId, out foldId))
                {
                    FolderTree folder;
                    string path = "";
                    string fpath = "";
                    if (FileType == "SAMPLEPROJECTFILE")
                    {
                        path = "Pages\\Project.csproj.txt";
                    }
                    else
                    {
                        var fld = from f in FolderTree.AllFolderTrees where f.Id == foldId select f;
                        folder = fld.First();
                        fpath = folder.FolderPath;
                        path = Path.Combine(fpath, FileName);
                        //path = path.Replace("\\\\", "\\");
                    }
                    if (FileType == "IMAGE")
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (Bitmap qrCodeImage = new Bitmap(path))
                            {
                                //string extension = Path.GetExtension(FileName);
                                var imgInput = System.Drawing.Image.FromFile(path);
                                var thisFormat = imgInput.RawFormat;
                                UInt32 width = (UInt32)imgInput.Width;
                                int height = imgInput.Height;
                                if (width > 800)
                                    width = 800;
                                qrCodeImage.Save(ms, thisFormat);// ImageFormat.Png);

                                text = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                                //text += Convert.ToBase64String(BitConverter.GetBytes(width));
                            }
                        }
                    }
                    else if (FileType == "ZIP")
                    {
                        if (!Directory.Exists(ZipFolder))
                        {
                            Directory.CreateDirectory(ZipFolder);
                        }
                        string zipPath = Path.Combine(ZipFolder,FileName);
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
                                file.Read(bytes, 0, (int)file.Length);
                                ms.Write(bytes, 0, (int)file.Length);
                            }

                            // Ref: https://docs.microsoft.com/en-us/dotnet/api/system.convert.tobase64string?view=netcore-3.1
                            // Look for example under ToBase64(Byte[], Base64FormattingOPetion)
                            // It does a Convert.ToBase64String followed by Covert.FromBas64String
                            // Note: No data type strings as used with images

                            text = Convert.ToBase64String(ms.ToArray());
                            //"application/zip"); ; ; ; // ;// ;

                        }
                    }
                    else
                    {
                        text = System.IO.File.ReadAllText(path);
                    }
                }
            }
            return text;
        }
    }
          
}
