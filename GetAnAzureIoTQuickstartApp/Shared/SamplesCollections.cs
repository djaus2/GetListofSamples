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

            //var projIds = (from p in Project.AllProjects select p.Id).ToList();
            //var treeIds = (from t in FolderTree.AllFolderTrees select t.Projects).ToList();
            //var ll = treeIds.SelectMany(d => d).ToList();

            //var sdf = ll.Except(projIds).ToList();
            //var sdf2 = projIds.Except(ll).ToList();
        }

    }
}
