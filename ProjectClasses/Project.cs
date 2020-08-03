using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectClasses
{
    public class Project
    {
        public static int Count = 0;
        public static List<Project> AllProjects=null;
        public int Id { get; set; }

        public Project()
        {
            if (AllProjects == null)
                AllProjects = new List<Project>();
            AllProjects.Add(this);
        }

        public Project(FolderTree folderTree)
        {
            Id = Count++;
            FolderTree = folderTree.Id;
            if (AllProjects == null)
                AllProjects = new List<Project>();
            AllProjects.Add(this);
        }

        int FolderTree { get; set; }


        public string GetPropertyValue(Project proj, string propertyName)
        {
            return (string) proj.GetType().GetProperties()
               .Single(pi => pi.Name == propertyName)
               .GetValue(proj, null);
        }
        public string Name { get; set; }
        public string Path { get; set; }
        private string relativePath;
        public string RelativePath {
            get { return relativePath; }
            set { relativePath = value;
                ProjectName = System.IO.Path.GetFileNameWithoutExtension(value);             
                folders = relativePath.Split('\\');
                ProjectFileName = Folders[Folders.Length - 1];
                Name = Folders[Folders.Length - 2];
                folders = Folders.Take(Folders.Length - 2).ToArray();
                depth = 0;
                root = "";
                sub = "";
                sub1 = "";
                sub2 = "";
                if (folders != null)
                {
                    depth = folders.Length;
                    if (folders.Length > 0)
                        root = Folders[0];
                    if (folders.Length > 1)
                        sub = Folders[1];
                    if (folders.Length > 2)
                        sub1 = Folders[2];
                    if (folders.Length > 3)
                        sub2 = Folders[3];
                }
            }
        }
        private string root;
        public string Root { 
            get {
                return root;
            }
        }
        private string sub = "";
        public string Sub {
            get
            {
                return sub;
            }
        }
        private string sub1 = "";
        public string Sub1
        {
            get
            {
                return sub1;
            }
        }
        private string sub2 = "";
        public string Sub2
        {
            get
            {
                return sub2;
            }
        }
        private int depth = 0;
        public int Depth
        {
            get
            {
                return depth;
            }
        }
        private string[] folders;
        public string[] Folders {
            get { return folders; }
        }



        public string ProjectName { get; set; }
        public string FullPath { get; set; }
        public string DeviceName { get; set; }
        public string ProjectFileName { get; set; }
        public string ProjectFileName_WithoutText
        {
            get
            {
                if (ProjectFileName.Substring(ProjectFileName.Length - ".txt".Length).ToLower() == ".txt")
                    return ProjectFileName.Substring(0, ProjectFileName.Length - ".txt".Length);
                else
                    return ProjectFileName;
            }
        }
        public string ProjectCSFileName { get; set; }
        public string ProjectCSFileName_WithoutText
        { 
            get {
                if (string.IsNullOrEmpty(ProjectCSFileName))
                    return "";
                else if (ProjectCSFileName.Length < 4)
                    return "";
                else if (ProjectCSFileName.Substring(ProjectCSFileName.Length - ".txt".Length).ToLower() == ".txt")
                    return ProjectCSFileName.Substring(0, ProjectCSFileName.Length - ".txt".Length);
                else
                    return ProjectCSFileName;
            } 
        }
        public string ProjectPNGFileName { get; set; }
        public int NoCSFiles { get; set; } = 1;
    }

    public class ProjectCount
    {
        public string DeviceName { get; set; }
        public int Count { get; set; } = 0;
    }
}
