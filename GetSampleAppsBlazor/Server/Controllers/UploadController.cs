using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System;

namespace GetSampleApps.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        public static string DefaultPath
        {
            get { return Shared.SamplesCollections.DefaultPath; }
        }
        public static string ServerUploadsFolder
        {
            get { return Shared.SamplesCollections.ServerUploadsFolder; }
        }
        public static string ServerZipFolder
        {
            get { return Shared.SamplesCollections.ServerZipFolder; }
        }
        public static string ServerSamplesFolder
        {
            get { return Shared.SamplesCollections.ServerSamplesFolder; }
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        private readonly IWebHostEnvironment environment;
        public UploadController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }


        [HttpPost]
        public async Task Post(IEnumerable<IFormFile> files)
        //[HttpPost]
        //public async Task<IActionResult> Post(IEnumerable<IFormFile> files) // the default field name. See SaveField
        {
            if (!Directory.Exists(ServerUploadsFolder))
            {
                Directory.CreateDirectory(ServerUploadsFolder);
            }
            if (files == null)
            { 
                 files = HttpContext.Request.Form.Files;
            }
            else if (files.Count() ==0)
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
                    var physicalPath = Path.Combine(ServerUploadsFolder, fileName);

                    // Implement security mechanisms here - prevent path traversals,
                    // check for allowed extensions, types, size, content, viruses, etc.
                    // this sample always saves the file to the root and is not sufficient for a real application

                    using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    if (Path.GetExtension(file.FileName).ToUpper() == ".ZIP")
                    {
                        if (Directory.Exists(ServerSamplesFolder))
                        {
                            Directory.Delete(ServerSamplesFolder, true);
                        }
                        Directory.CreateDirectory(ServerSamplesFolder);
                        // Extract it to ./Repository
                        ZipFile.ExtractToDirectory(physicalPath, ServerSamplesFolder);
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


            // Return an empty string message in this case
            ;
        }



        //[HttpPost]
        //public async Task Post()
        //{
        //    if (!Directory.Exists(UploadFolder))
        //    {
        //        Directory.CreateDirectory(UploadFolder);
        //    }
        //    //if (Directory.Exists(RepositoryFolder))
        //    //{
        //    //    Directory.Delete(RepositoryFolder, true);
        //    //}
        //    Directory.CreateDirectory(RepositoryFolder);
        //    if (HttpContext.Request.Form.Files.Any())
        //    {
        //        bool IsFirst = true;
        //        foreach (var file in HttpContext.Request.Form.Files)
        //        {
        //            if (IsFirst)
        //            {
        //                IsFirst = false;
        //                System.Diagnostics.Debug.WriteLine("=== SVR File {0}", file.Length);
        //                var path = Path.Combine(environment.ContentRootPath, UploadFolder, file.FileName);
        //                using (var stream = new FileStream(path, FileMode.Create))
        //                {
        //                    await file.CopyToAsync(stream);
        //                }
        //                FileInfo fi = new FileInfo(path);
        //                System.Diagnostics.Debug.WriteLine(" === SVR FileSize {0}", fi.Length);
        //                //if (Path.GetExtension(file.FileName).ToUpper() == ".ZIP")
        //                //{
        //                //    // Extract it to ./Repository
        //                //    ZipFile.ExtractToDirectory(path, RepositoryFolder);
        //                //}
        //                break;
        //            }
        //        }
        //    }
        //}
    }

    //[ Route("controller")]
    //public class SaveController : ControllerBase
    //{
    //    private readonly IWebHostEnvironment environment;
    //    public SaveController(IWebHostEnvironment environment)
    //    {
    //        this.environment = environment;
    //    }
    //    [HttpPost]
    //    public async Task<IActionResult> Save()
    //    {
    //        var tempFileName = Path.Combine(environment.ContentRootPath, ".//Uploads", "one.zip");
    //        //var tempFileName = Path.GetTempFileName();
    //        using (var writer = System.IO.File.OpenWrite(tempFileName))
    //        {
    //            await Request.Body.CopyToAsync(writer);
    //        }
    //        return Ok(Path.GetFileNameWithoutExtension(tempFileName));
    //    }
    //}
}
