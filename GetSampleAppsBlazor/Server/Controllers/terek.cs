using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GetSampleApps.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    public class UploadControllerT : Controller
    {
        public IWebHostEnvironment HostingEnvironment { get; set; }

        public UploadControllerT(IWebHostEnvironment hostingEnvironment)
        {
            HostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Save(IEnumerable<IFormFile> files) // the default field name. See SaveField
        {
            if (files != null)
            {
                try
                {
                    foreach (var file in files)
                    {
                        var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                        var physicalPath = Path.Combine(HostingEnvironment.WebRootPath, fileName);

                        // Implement security mechanisms here - prevent path traversals,
                        // check for allowed extensions, types, size, content, viruses, etc.
                        // this sample always saves the file to the root and is not sufficient for a real application

                        using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                    }
                }
                catch
                {
                    // implement error handling here, this merely indicates a failure to the upload
                    Response.StatusCode = 500;
                    await Response.WriteAsync("some error message"); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }


        [HttpPost]
        public ActionResult Remove(string[] files) // the default field name. See RemoveField
        {
            if (files != null)
            {
                try
                {
                    foreach (var fullName in files)
                    {
                        var fileName = Path.GetFileName(fullName);
                        var physicalPath = Path.Combine(HostingEnvironment.WebRootPath, fileName);

                        if (System.IO.File.Exists(physicalPath))
                        {
                            // Implement security mechanisms here - prevent path traversals,
                            // check for allowed extensions, types, permissions, etc.
                            // this sample always deletes the file from the root and is not sufficient for a real application

                            System.IO.File.Delete(physicalPath);
                        }
                    }
                }
                catch
                {
                    // implement error handling here, this merely indicates a failure to the upload
                    Response.StatusCode = 500;
                    Response.WriteAsync("some error message"); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }
    }
}
