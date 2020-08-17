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
using System;

namespace GetSampleApps.Server
{
    public class Startup
    {
        public static void Setup(int pathToUse )
        {
            string defaultPath = GetSampleApps.Shared.SamplesCollections.SetSamplesFolder(pathToUse);
            if (defaultPath[0] != '!')
            {
                var rootSample = GetSamples.GetSamplesProjects.GetFolders(defaultPath);
                GetSampleApps.Shared.SamplesCollections.GetFolderandProjectLists(rootSample);
                System.Diagnostics.Debug.WriteLine("*********");
                System.Diagnostics.Debug.WriteLine(Project.AllProjects.Count);
                System.Diagnostics.Debug.WriteLine("*********");
                System.Diagnostics.Debug.WriteLine(FolderTree.AllFolderTrees.Count);
                System.Diagnostics.Debug.WriteLine("*********");
            }
        }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // In appsettings.json:

            int PathToUse  = Configuration.GetValue<int>("PathToUse");
            string DevPathToRepository = Configuration.GetValue<string>("DevPathToRepository");
            string ServerSampesFolder = Configuration.GetValue<string>("ServerSampesFolder");
            string ServerZipFolder = Configuration.GetValue<string>("ServerZipFolder");
            string ServerUploadsFolder = Configuration.GetValue<string>("ServerUploadsFolder");
            string ServerSamplesFolder = Configuration.GetValue<string>("ServerSamplesFolder");

            GetSampleApps.Shared.SamplesCollections.PathToUse = PathToUse;
            GetSampleApps.Shared.SamplesCollections.ServerZipFolder = ServerZipFolder;
            GetSampleApps.Shared.SamplesCollections.ServerZipFolder = ServerZipFolder;
            GetSampleApps.Shared.SamplesCollections.ServerUploadsFolder = ServerUploadsFolder;
            GetSampleApps.Shared.SamplesCollections.DevPathToRepository = DevPathToRepository;

            Setup(PathToUse);

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
