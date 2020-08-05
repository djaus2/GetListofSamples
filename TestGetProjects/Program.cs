using Microsoft.Extensions.Configuration;
using ProjectClasses;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TestGetProjects
{
    class Program
    {
        static void Main(string[] args)
        {

            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //.AddUserSecrets<Program>()
            //.AddEnvironmentVariables();
            Microsoft.Extensions.Configuration.IConfiguration Configuration = builder.Build();
            Console.WriteLine("Hello World!");
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

            string GenerateTextPath = Configuration.GetValue<string>("GenerateTextPath");

            var rootFolder = GetSamples.GetSamplesProjects.GetFolders(DefaultPath, GenerateTextPath);


            System.Diagnostics.Debug.WriteLine(Project.AllProjects.Count);

            foreach (var p in Project.AllProjects)
            {
                Console.WriteLine("{0}  {2}  {4}  {6}  {8}  {10}",p.Root, " - ", p.Sub, " - ", p.Sub1, " - ", p.Sub2, " - ", p.ProjectName, " - ", p.ProjectFileName);
            }

            foreach (var t in FolderTree.AllFolderTrees)
            {
                if (t.HasProjects)
                    Console.WriteLine("{0} \tId:{1} \tParent:{2} \tNumChildren:{3} \tNumProjects:{4}", t.FolderName, t.Id, t.Parent, t.NumChildren, t.NumProjects);
            }

            foreach (var t in FolderTree.AllFolderTrees)
            {
                if (t.HasSolutions)
                    Console.WriteLine("{0} \tId:{1} \tParent:{2} \tNumSolutions:{3} \tSolution{4}", t.FolderName, t.Id, t.Parent, t.NumSolutions, t.Solutions[0]);
            }

            var projIds = (from p in Project.AllProjects select p.Id).ToList();
            var treeIds = (from t in FolderTree.AllFolderTrees select t.Projects).ToList();
            var ll = treeIds.SelectMany(d => d).ToList();

            var sdf = ll.Except(projIds).ToList();
            var sdf2 = projIds.Except(ll).ToList();


        }

   
    }
}
