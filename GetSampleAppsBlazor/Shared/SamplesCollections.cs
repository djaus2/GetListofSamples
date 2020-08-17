using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using ProjectClasses;

namespace GetSampleApps.Shared
{

    public class SamplesCollections
    {
        public static List<FolderTree> Folders { get; set; }
        public static List<Project> Projects { get; set; }
        public static FolderTree RootFolder { get; set; }
        public static int PathToUse { get; set; }
        public static string ServerZipFolder { get; set; }
        public static string ServerUploadsFolder { get; set; }
        public static string ServerSamplesFolder { get; set; }
        public static string DefaultPath { get; set; }
        public static string DevPathToRepository { get; set; }

        public static string SetSamplesFolder(int pathToUse)
        {
            PathToUse = pathToUse;
            DefaultPath = "";
            switch (PathToUse)
            {
                case 1:
                    if (Directory.Exists(DevPathToRepository))
                        DefaultPath = DevPathToRepository;
                    break;
                case 2:
                    if (Directory.Exists(ServerSamplesFolder))
                        DefaultPath = ServerSamplesFolder;
                    break;
                default:
                    // If not 1 or 2 search
                    bool done = false;
                    // If there is a zip file in uploads and nothing repository folder
                    // Extract the first zipfile to the repository file
                    if (Directory.Exists(ServerSamplesFolder))
                    {
                        // A bit apriori but if repository folder has subdirectories, 
                        // assume it is a samples foler.
                        var dirs2 = Directory.GetDirectories(ServerSamplesFolder);
                        if (dirs2.Count() > 0)
                        {
                            DefaultPath = ServerSamplesFolder;
                            System.Diagnostics.Debug.WriteLine("Using existing Folder on Server");
                            done = true;
                        }
                    }
                    if (!done)
                    {
                        if (Directory.Exists(ServerUploadsFolder))
                        {                
                            var zipFiles = Directory.GetFiles(ServerUploadsFolder, "*.zip");
                            if (zipFiles.Count() != 0)
                            {
                                var zipPath = "";
                                // [1] Look for Samples.zip
                                var sampleFiles = Directory.GetFiles(ServerUploadsFolder, "Samples.zip");
                                if (sampleFiles.Count() != 0)
                                {
                                    zipPath = sampleFiles.First();
                                }
                                else
                                {
                                    // [2[ Otherwise use first zip file
                                    zipPath = zipFiles.First();
                                }
                                if (Directory.Exists(ServerSamplesFolder))
                                {
                                    Directory.Delete(ServerSamplesFolder, true);
                                }
                                Directory.CreateDirectory(ServerSamplesFolder);
                                // Extract it to ./Samples
                                System.Diagnostics.Debug.WriteLine("Generating Samples from zipfile.");
                                ZipFile.ExtractToDirectory(zipPath, ServerSamplesFolder);
                                DefaultPath = ServerSamplesFolder;
                                done = true;
                            }                       
                        }
                    }
                    if (!done)
                    {
                        System.Diagnostics.Debug.WriteLine("Using appdesttings.json \"DevPathToRepository\" setting for Sanmples: {0}", DevPathToRepository);
                        DefaultPath = DevPathToRepository; 
                    }
                    break;
            }
            // This should always fail but here as a check.
            if (!Directory.Exists(DefaultPath))
                DefaultPath = DevPathToRepository;
            if (!Directory.Exists(DefaultPath))
            {
                DefaultPath = "! Samples folder not found";
                System.Diagnostics.Debug.WriteLine("Samples folder not found.");
                return DefaultPath;
            }

            var dirs = Directory.GetDirectories(DefaultPath);
            if (dirs.Count() == 0)
            {
                DefaultPath = "! No Samples in folder";
                System.Diagnostics.Debug.WriteLine("No Samples in Folder");
                return DefaultPath;
            }
            System.Diagnostics.Debug.WriteLine($"Using {DefaultPath} for Samples");
            return DefaultPath;
        }

        public static void GetFolderandProjectLists(FolderTree rootFolder)
        {
            RootFolder = rootFolder;
            Folders = FolderTree.AllFolderTrees;
            Projects = Project.AllProjects;
        }

    }
}
