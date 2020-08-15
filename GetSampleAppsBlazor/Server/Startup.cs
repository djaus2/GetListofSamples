using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using ProjectClasses;
using System.IO;
using System.IO.Compression;

namespace GetSampleApps.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // In appsettings.json:

            int PathToUse12or3 = Configuration.GetValue<int>("PathToUse12or3");
            string DefaultPath = "";
            switch (PathToUse12or3)
            {
                case 1:
                    DefaultPath = Configuration.GetValue<string>("PathToRepository");
                    break;
                case 2:
                    DefaultPath = Configuration.GetValue<string>("PathToRepositoryAlt");
                    break;
                case 3:
                    DefaultPath = Configuration.GetValue<string>("PathToRepositoryWithinServer");
                    break;
                case 4:
                    DefaultPath = Configuration.GetValue<string>("PathToRepositoryCopy");
                    break;
            }


            string ZipFolder = Configuration.GetValue<string>("ZipFolder");
            string UploadFolder = Configuration.GetValue<string>("UploadFolder");
            string SamplesFolder = Configuration.GetValue<string>("SamplesFolder");
            string GenerateTextPath = Configuration.GetValue<string>("GenerateTextPath");

            bool done = false;
            // If there is a zip file in uploads and nothing repository folder
            // Extract the first zipfile to the repository file
            if (Directory.Exists(UploadFolder))
            {
                if (Directory.Exists(SamplesFolder))
                {
                    // A bit apriori but if repository folder ahs subdirectories, 
                    // assume it is a samples foler.
                    var dirs = Directory.GetDirectories(SamplesFolder);
                    if (dirs.Count() > 0)
                    {
                        DefaultPath = SamplesFolder;
                        System.Diagnostics.Debug.WriteLine("Using existing Folder");
                        done = true;
                    }
                }
                if (!done)
                {
                    var zipFiles = Directory.GetFiles(UploadFolder, "*.zip");
                    if (zipFiles.Count() != 0)
                    {
                        var zipfile = zipFiles.First();
                        if (Directory.Exists(SamplesFolder))
                        {
                            Directory.Delete(SamplesFolder, true);
                        }
                        Directory.CreateDirectory(SamplesFolder);
                        // Extract it to ./Samples
                        ZipFile.ExtractToDirectory(zipfile, SamplesFolder);
                        DefaultPath = SamplesFolder;
                        System.Diagnostics.Debug.WriteLine("Generating Samples from zipfile.");
                        done = true;
                    }
                }
            }
            else 
            {
                if (Directory.Exists(SamplesFolder))
                {
                    // A bit apriori but if repository folder ahs subdirectories, 
                    // assume it is a samples foler.
                    var dirs = Directory.GetDirectories(SamplesFolder);
                    if (dirs.Count() > 0)
                    {
                        DefaultPath = SamplesFolder;
                        System.Diagnostics.Debug.WriteLine("Using existing Folder");
                        done = true;
                    }
                }
            }
            if (!done)
            {
                System.Diagnostics.Debug.WriteLine("Using appdesttings.json \"PathToRepository\" setting for Sanmples: {0}", DefaultPath);
            }
        

            // Used for rescanning folders.
            Controllers.SamplesController.ZipFolder = ZipFolder;
            Controllers.SamplesController.UploadFolder = UploadFolder;
            Controllers.SamplesController.SamplesFolder = SamplesFolder;
            Controllers.SamplesController.DefaultPath = DefaultPath;
            Controllers.SamplesController.GenerateTextPath = GenerateTextPath;
            Controllers.UploadController.ZipFolder = ZipFolder;
            Controllers.UploadController.UploadFolder = UploadFolder;
            Controllers.UploadController.SamplesFolder = SamplesFolder;
            
            var rootSample = GetSamples.GetSamplesProjects.GetFolders(DefaultPath, GenerateTextPath);

            System.Diagnostics.Debug.WriteLine("*********");
            System.Diagnostics.Debug.WriteLine(Project.AllProjects.Count);
            System.Diagnostics.Debug.WriteLine("*********");
            System.Diagnostics.Debug.WriteLine(FolderTree.AllFolderTrees.Count);
            System.Diagnostics.Debug.WriteLine("*********");
            //foreach (var t in FolderTree.AllFolderTrees)
            //{
            //    System.Diagnostics.Debug.WriteLine("{0} \tId:{1} \tDepth:{2} \tParent:{3} \tNumChildren:{4} \tNumProjects:{5}", t.FolderName, t.Id, t.Depth, t.Parent, t.NumChildren, t.NumProjects);
            //}

            GetSampleApps.Shared.SamplesCollections.Init(
                rootSample
            );
        }
        

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
