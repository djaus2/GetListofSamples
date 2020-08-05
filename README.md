# .NET IoT SDK Samples

Working on project to generically recurse a folder for sample C# apps  
Displays in folding layout.  
Tags Solutions and Projects.

## Implementation

Uses a recursive Component that calls itself. 
That way the app can recursively drill into folder information.
  
Server at startup recurses specified folder collecting data 
about folders and project/solutions contained therein.
  
Data Structures:
- A FolderTree structure (Id, Parent, Children, Projects and Solution properties ) 
- Project (Various properties such as path, and an Id)
- List of FolderTrees
- List of Projects
- FolderTree Ids used in Parent and Children property lists
- Project Ids used in FolderTree Project lists

When required, the complete list of FolderTrees and and Projects is sent to the client from the server.  
Using Ids in list simplifies passing of data over Http.

Requires the path to the folder in server appsettings.json  
Set for .NET IoT SDK repository



