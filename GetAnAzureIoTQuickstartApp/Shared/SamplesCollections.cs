using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectClasses;

namespace GetAnAzureIoTQuickstartApp.Shared
{

    public class SamplesCollections
    {
        public static List<FolderTree> Folders { get; set; }
        public static List<Project> Projects { get; set; }
        public static FolderTree RootFolder { get; set; }

        public static void Init(FolderTree rootFolder)
        {
            RootFolder = rootFolder;
            Folders = FolderTree.AllFolderTrees;
            Projects = Project.AllProjects;
        }

    }
}
