using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.IO.Compression;

namespace GetSampleApps.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        public static string UploadFolder { get; set; } = "";
        public static string RepositoryFolder { get; set; } = "";
        private readonly IWebHostEnvironment environment;
        public UploadController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }


        [HttpPost]
        public async Task Post()
        {
            if (!Directory.Exists(UploadFolder))
            {
                Directory.CreateDirectory(UploadFolder);
            }
            if (Directory.Exists(RepositoryFolder))
            {
                Directory.Delete(RepositoryFolder, true);
            }
            Directory.CreateDirectory(RepositoryFolder);
            if (HttpContext.Request.Form.Files.Any())
            {
                foreach (var file in HttpContext.Request.Form.Files)
                {
                    var path = Path.Combine(environment.ContentRootPath, UploadFolder, file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    if (Path.GetExtension(file.FileName).ToUpper() == ".ZIP")
                    {
                        // Extract it to ./Repository
                        ZipFile.ExtractToDirectory(path, RepositoryFolder);
                    }
                }
            }
        }
    }
}
