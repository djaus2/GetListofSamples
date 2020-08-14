# .NET C# Sample Projects Indexor

A Project to generically recurse a folder for sample C# apps information. 
Displays the information in folding tree layout.  
Tags Solutions and Projects.

<b><i> See Finale below. </i><b>

## Implementation

Uses a recursive Component that calls itself. 
That way the app can recursively drill into folder information.
  
Folders now expand and contract when clicked.  
When a folder is expanded all of its siblings close.
  
Server at startup recurses specified folder collecting data 
about folders and project/solutions contained therein.
  
Data Structures:
- A FolderTree structure (Id, Parent, Children, Projects, Solution, ReadMe and Image properties ) 
- Project (Various properties such as path, and an Id)
- List of FolderTrees
- List of Projects
- List of Markdown files
- List of Images
- FolderTree Ids used in Parent and Children property lists
- Project Ids used in FolderTree Project lists

When required, the complete list of FolderTrees and and Projects is sent to the client from the server.  
Using Ids in list simplifies passing of data over Http.  
Lists are turned into Dictionary using Id as key sio can be used as a spare array.

Requires the path to the folder in server appsettings.json  
Set for .NET IoT SDK repository

## Custom Razor Components
- FolderTree that recurses specified folder on server for files in folders
  - Is recursive in that it calls itself.
  - Is a Folding UI component.
- Display Markdown text listed in folder.
- Zip folder on server and download
- Download text (.cs or .csproj) on server and display
  - Can then copy or download it
- Display an image listed in folder
- Might add a file (zip) upload so can remotely update the folders.
  - Can now do that (File upload). If zip file then gets unzipped (Needs testing)

## Finale!

<b><i> Can now upload a zipped folder of samples, which get extracted to the server's repository folder,  
  then scan it, upload the new data to the client and then use those samples.</i><b>



