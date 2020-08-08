using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using ProjectClasses;

namespace GetSamples
{
    public static class GetSamplesProjects
    {
        public static string GenerateTextPath = "c:\\temp\\dotnet";
        //const string DefaultPath = @"C:\Users\DavidJones\source\dotnet\djaus2-2-iot\iot\src\Devices";
        public static  string DefaultPath =  @"C:\Users\DavidJones\source\dotnet-djaus-iot\iot";


        public static FolderTree GetFolders(string path, string generateTextPath)
        {
            GenerateTextPath = generateTextPath;
            if (path == "")
                path = DefaultPath;
            var rootFolder = GetTree(path,null);
            
            System.Diagnostics.Debug.WriteLine("=======");
            System.Diagnostics.Debug.WriteLine(Project.AllProjects.Count());
            System.Diagnostics.Debug.WriteLine("=======");
            System.Diagnostics.Debug.WriteLine(FolderTree.AllFolderTrees.Count());
            return rootFolder;
        }


        public static FolderTree GetTree(string path, FolderTree parent)
        {
            FolderTree Folder = new FolderTree(parent);
            Folder.FolderPath = path;

            var fldrs = path.Split(new char[] { '\\' });
            string folderName = fldrs[fldrs.Length - 1];

            string[] projFilePaths = Directory.GetFiles(path, "*.csproj");
            foreach (var fp in projFilePaths)
            {
                var localPath = fp.Replace(@"C:\Users\david\Downloads\azure-iot-samples-csharp-master_src\azure-iot-samples-csharp-master\", "");

                Project proj = new Project(Folder)
                {
                    RelativePath = localPath,
                    FullPath = fp
                };
                //Assuming one project per folder. Not true for dotnet/iot
                string[] csFiles = Directory.GetFiles(path, "*.cs");
                foreach (var cs in csFiles)
                {
                    var cslnlocalPath = cs.Replace(@"C:\Users\david\Downloads\azure-iot-samples-csharp-master_src\azure-iot-samples-csharp-master\", "");

                    var csShort = Path.GetFileName(cs);
                    proj.ProjectCSFileNames.Add(csShort);
                }
                Folder.Projects.Add(proj.Id);
            }

            string[] solutions = Directory.GetFiles(path, "*.sln");
            foreach (var sln in solutions)
            {
                var slnlocalPath = sln.Replace(@"C:\Users\david\Downloads\azure-iot-samples-csharp-master_src\azure-iot-samples-csharp-master\", "");


                Folder.Solutions.Add(sln);
                //Folder.LocalSolutions.Add(slnlocalPath);
            }

            string[] readmes = Directory.GetFiles(path, "*.md");
            foreach (var readme in readmes)
            {
                //var slnlocalPath = sln.Replace(@"C:\Users\david\Downloads\azure-iot-samples-csharp-master_src\azure-iot-samples-csharp-master\", "");

                string readmeShort = Path.GetFileName(readme);
                Folder.ReadMes.Add(readmeShort);
                //Folder.LocalSolutions.Add(slnlocalPath);
            }



            string[] dirs = Directory.GetDirectories(
                    path);
            foreach (string dir in dirs)
            {
                var fldrs2 = dir.Split(new char[] { '\\' });
                string folderName2= fldrs2[fldrs2.Length - 1];
                FolderTree  folder = GetTree(dir, Folder);
                if (folder != null)
                {
                    if ((folder.Children.Count != 0) || (folder.Projects.Count != 0))
                        Folder.Children.Add(folder.Id);
                }
            }
            if ((Folder.Children.Count==0)&& (Folder.Projects.Count==0) && (Folder.Solutions.Count == 0))
            {
                FolderTree.AllFolderTrees.Remove(Folder);
                Folder = null;
            }
            return Folder;
        }

    }
}

