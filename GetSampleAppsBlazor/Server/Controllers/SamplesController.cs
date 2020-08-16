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
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GetSampleApps.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplesController : ControllerBase
    {
        /// <summary>
        /// Thes are set by Server startup, read from appsettings.json
        /// </summary>
        public static string DefaultPath { get; set; } = "";
        public static string UploadFolder { get; set; } = "";
        public static string ZipFolder { get; set; } = "";
        public static string SamplesFolder { get; set; } = "";
        public static string GenerateTextPath { get; set; } = "";
        ////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Send both of the Projects and Folders lists as one text stream.
        /// Stream is two json files, one for each of Projects and Folders separated by a '`' character.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string Get()
        {

            string json = "";
            var projects = SamplesCollections.Projects;
            string json1 = JsonConvert.SerializeObject(projects);

            var folders = SamplesCollections.Folders;
            string json2 =  JsonConvert.SerializeObject(folders);
            json = json2 + "~" + json1;
            return json;
        }

        // Action various GET commmands
        [HttpGet("{param}")]
        public string Get(string param)
        {
            string text = "404";
            System.Diagnostics.Debug.WriteLine(param);
            string Command = "";
            string FolderId = "";
            string FileName = "";
            int foldId;

            string[] names = param.Split(new char[] { '~' });
            Command = names[0];
            Command = Command.ToUpper();
            if (names.Length > 1)
            {
                FileName = names[1];
                if (names.Length > 2)
                {
                    FolderId = names[2];
                }
            }
            

            if (Command == "GETZIPS")
            {
                // Get ~ separated list of zip files in UploadsFolder
                string Zips = "None";
                if (Directory.Exists(UploadFolder))
                {
                    var zips = Directory.GetFiles(UploadFolder, "*.zip");
                    var zipFilenames = from z in zips select Path.GetFileName(z);
                    Zips = String.Join("~", zipFilenames);
                }
                text = Zips;
            }
            else if (Command == "UNZIP")
            {
                /// Unzip an existing zip file in UploadsFolder to SamplesFolder
                string SamplesZipFilename;
                text = "404, Zip File Not found.";
                if (names.Length > 3)
                {
                    if (!string.IsNullOrEmpty(names[3]))
                    {
                        {
                            SamplesZipFilename = names[3];
                            if (Directory.Exists(UploadFolder))
                            {
                                string filePath = Path.Combine(UploadFolder, SamplesZipFilename);
                                if (System.IO.File.Exists(filePath))
                                {
                                    if (Directory.Exists(SamplesFolder))
                                    {
                                        Directory.Delete(SamplesFolder, true);
                                    }
                                    Directory.CreateDirectory(SamplesFolder);
                                    ZipFile.ExtractToDirectory(filePath, SamplesFolder);
                                    text = $"200,{SamplesZipFilename} extracted to {SamplesFolder}";
                                }
                            }
                        }
                    }
                }
            }
            else if (Command=="RELOAD")
            {
                /// Rescan the Samples folder
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
                    if (Command == "SAMPLEPROJECTFILE")
                    {
                        path = "Pages\\Project.csproj.txt";
                    }
                    else
                    {
                        var fld = from f in FolderTree.AllFolderTrees where f.Id == foldId select f;
                        folder = fld.First();
                        fpath = folder.FolderPath;
                        path = Path.Combine(fpath, FileName);
                    }

                    ////////////////////////////////////////
                    
                    if (Command == "IMAGE")
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
                    else if (Command == "ZIP")
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

        /// <summary>
        /// Delete the contents of the various content fies on the server
        /// </summary>
        /// <param name="FolderId">1,2 or 3 for Zips(Downloads), Uploadss (Samples zips) and The Samples forlder.</param>
        /// <returns>Http Status</returns>
        [HttpDelete("{FolderId}")]
        public async Task Delete(string FolderId)
        {
            string text = "";
            string folder = "skip";
            int ResponseStatus = 200; //OK
            try
            {
                switch (FolderId)
                {
                    case "1":
                        folder = ZipFolder;
                        break;
                    case "2":
                        folder = UploadFolder;
                        break;
                    case "3":
                        folder = SamplesFolder;
                        break;
                }
                if (folder != "skip")
                {
                    if (Directory.Exists(folder))
                    {
                        Directory.Delete(folder,true);
                    }
                    Directory.CreateDirectory(folder);
                    text = "OK";
                }              
                else
                {
                    ResponseStatus = 404;
                    text = "Id doesn't correspond to a folder (Valid Ids:1..3)";
                }
            } catch (Exception ex)
            {
                Response.StatusCode = 500;
                text = ex.Message;
            }
            Response.StatusCode = ResponseStatus;
            await Response.WriteAsync(text);
        }

        /// <summary>
        /// Upload a file to UploadsFolder and extract to SamplesFolder if a zip file.
        /// </summary>
        /// <param name="files">The streamed file data from the client.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task Post(IEnumerable<IFormFile> files)
        {
            if (!Directory.Exists(UploadFolder))
            {
                Directory.CreateDirectory(UploadFolder);
            }
            if (files == null)
            {
                files = HttpContext.Request.Form.Files;
            }
            else if (files.Count() == 0)
            {
                files = HttpContext.Request.Form.Files;
            }
            try
            {
                foreach (var file in files)
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    System.Diagnostics.Debug.WriteLine("=== SVR Content {0}", fileContent.Size);
                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine(UploadFolder, fileName);

                    // Implement security mechanisms here - prevent path traversals,
                    // check for allowed extensions, types, size, content, viruses, etc.
                    // this sample always saves the file to the root and is not sufficient for a real application

                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (Path.GetExtension(file.FileName).ToUpper() == ".ZIP")
                    {
                        if (Directory.Exists(SamplesFolder))
                        {
                            Directory.Delete(SamplesFolder, true);
                        }
                        Directory.CreateDirectory(SamplesFolder);
                        // Extract it to ./Repository
                        ZipFile.ExtractToDirectory(physicalPath, SamplesFolder);
                    }

                    FileInfo fi = new FileInfo(physicalPath);
                    System.Diagnostics.Debug.WriteLine(" === SVR FileSize {0}", fi.Length);
                    Response.StatusCode = 200;
                    string msg = $"File Uploaded:{fi.Name}     Size:{fi.Length }";
                    await Response.WriteAsync(msg); // custom error message
                }
            }
            catch (Exception ex)
            {
                // implement error handling here, this merely indicates a failure to the upload
                Response.StatusCode = 500;
                await Response.WriteAsync(ex.Message); // custom error message
            }

        }

    }

}
