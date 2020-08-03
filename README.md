# dotnet_iot_samples

Working on project to recurse dotnet/iot/src/Devices/<Device Names>/samples directories and get a list and project info. **_Done_**


The intention is for user to select a sample project and to either build and remotely deploy the  sample, or or to deploy the project and source files to the remote location for building there.

Requires the dotnet/iot  or djaus2/iot repository tree to be present locally.  
Set this in the server's appsettings.json

App is ready to use now.  Includes resolution of Packages versioning issue. See [Nuget Packages 101: Creating and using a local Nuget Package](http://www.sportronics.com.au/coding/Nuget_Packages-Creating_and_using_a_local_Package-coding.html)

## Use
To use the app, get this repository and one of dotnet/iot or djaus2/iot _(in a separate folder)_. Set the app to point to the second repository in appsettings.json. Note there is a property there which takes on values 1,2 or 3 to point to different locations. Currently it is set to 3 which points to dotnet\iot in the server. Don't put your copy of the iot repository there as you will get issues with .cs and csproj being compiled. Also for remote use you will need to change localhost the IPAddress of where the server runs, in Propeties\launchsettings.json

This app is currently hosted on azure at [getiotsamples.azurewebsites.net](https://getiotsamples.azurewebsites.net/).  
- All projects are there now.  
- Limitation: Only single source code sample projects supported.

