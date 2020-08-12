using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using ProjectClasses;

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
            string RepositoryFolder = Configuration.GetValue<string>("RepositoryFolder");

            string GenerateTextPath = Configuration.GetValue<string>("GenerateTextPath");

            // Used for rescanning folders.
            Controllers.SamplesController.ZipFolder = ZipFolder;
            Controllers.SamplesController.DefaultPath = DefaultPath;
            Controllers.SamplesController.GenerateTextPath = GenerateTextPath;
            Controllers.UploadController.UploadFolder = UploadFolder;
            Controllers.UploadController.RepositoryFolder = RepositoryFolder;

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
