using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;

namespace ProjectClasses
{
    public class FolderTree
    {
        public static int Count = 0;
        public static List<FolderTree> AllFolderTrees=null;

        public int Id { get; set; }

        public FolderTree( FolderTree parent)
        {
            if (parent == null)
            {
                Parent = -1;
                Count = 0;
            }
            else
            {
                Parent = parent.Id;
            }
            Id = Count++;
            Children = new List<int>();
            Projects = new List<int>();
            if (AllFolderTrees == null)
                AllFolderTrees = new List<FolderTree>();
            AllFolderTrees.Add(this);
        }

        public FolderTree()
        {
            if (AllFolderTrees == null)
                AllFolderTrees = new List<FolderTree>();
            AllFolderTrees.Add(this);
        }

        public int Parent { get; set; } = -1;
        public List<int> Children { get; set; }
        public List<int> Projects { get; set; }

        [JsonIgnore]
        public List<string> ProjectNames
        {
            get
            { 
                var projNames = from p in Project.AllProjects where Projects.Contains(p.Id) select p.Name;
                return projNames.ToList();
            }
        }
        [JsonIgnore]
        public int Depth
        {
            get
            {
                if (Parent == -1)
                    return 0;
                else
                    return AllFolderTrees[Parent].Depth + 1;
            }
        }

        public string FolderName { get; set; }
        string folderPath;
        public string FolderPath { 
            get { return folderPath; }
            set
            {
                folderPath = value;
                FolderName =  Path.GetFileName(folderPath);    
            }
        }
        [JsonIgnore]
        public int NumProjects {
            get{
                return Projects.Count;
            }
        }
        [JsonIgnore]
        public int NumChildren
        {
            get
            {
                return Children.Count;
            }
        }
        [JsonIgnore]
        public bool HasChildren
        {
            get
            {
                return (NumChildren != 0);
            }
        }
        [JsonIgnore]
        public bool HasProjects
        {
            get
            {
                return (NumProjects != 0);
            }
        }

 
    }
}
