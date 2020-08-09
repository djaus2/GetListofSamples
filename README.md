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
- Display Markdown text
- Zip folder on server and download
- Download text (.cs or .csproj) on server and display
  - Can then copy or download it
- Display an image in folder

## Non Breaking Assumptions:
- Only one or less image files in folder. Ignores all bar first
- Only one .md or less in folder. Ignores all bar first

## What If
- More than one solution file in a folder: Lists all
- More than one project file in a folder: Lists all
  - Nb: Download of zip downloads all files in a folder (and folders below)


