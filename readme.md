# ELI

## Requirements

### Windows

 - .NET Core 2.1 SDK
 - Visual Studio 2017 (version 15.7+)

### Mac

 - .NET Core 2.1 SDK
 - Visual Studio for Mac (version 7.5+)

## Development notes

We are trying to make this project one that can be worked on by folks using Mac or Windows, so please be mindful that you are not checking in Windows-specific config files that do not work on a Mac.

### Using local IIS on Windows to serve project without publishing

The project can be run from within Visual Studio using IIS Express.  However, depending on your development setup/work process, you may want to serve the project via your local IIS.  By default in .NET Core, this requires publishing, then setting up the project in IIS which is...not ideal.  Ideally, one wants to just build the project, then be able to view the app at the standard location for your local IIS setup. Good news - it's possible!

1. Install the AspNetCoreModule.

	- Open Visual Studio Installer. Select "Modify" for the Visual Studio instance you're using.
	- Under Installation Details > ASP.NET and web development, select `Development time IIS support`. Then click the modify button. This will install the AspNetCoreModule.

2. Add a web.config file to the base of the ELI project folder. 

  Example below. Update the arguments path to point to location of the ELI project in your local ELI solution folder.

  ```
  <?xml version="1.0" encoding="utf-8"?>  <configuration>    <!-- To customize the asp.net core module uncomment and edit the following section.   For more info see https://go.microsoft.com/fwlink/?linkid=838655 -->      <system.webServer>      <handlers>        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModule" resourceType="Unspecified" />      </handlers>      <aspNetCore processPath="C:\Program Files\dotnet\dotnet.exe"                arguments="run --project C:\Path\to\your\eli\ELI"                 stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" forwardWindowsAuthToken="false">          <environmentVariables>            <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Development" />          </environmentVariables>      </aspNetCore>    </system.webServer>    </configuration>
  ```

3. Set up the app in IIS.  Set up the app as your normally would in IIS. For the application pool, set the .NET CLR version to No Managed Code.

### Using integrated authentication to connect to database (for development on Windows with IIS)

You can use integrated authentication with .NET Core projects to authenticate and connect to a database.  As with other projects, you will want to set the identity of the application pool to a user that has access to the database (your account or a service account).  You may also need to provide this user read access to the directory where your `dotnet.exe` is (likely `C:\Program Files\dotnet\`).

Any updates you make to the connection string should be done in an environment specific appsettings file like `appsettings.Development.json` _not_ in the main `appsettings.json`. Refer to docs for a sample file.

### Database authentication for Mac users

There is not yet a method for Mac users to connect to the database.  Currently the only solution is integrated authentication which only works on Windows.  But I will work on this soon.

#### More information

 - [https://stackoverflow.com/a/50286082/891018](https://stackoverflow.com/a/50286082/891018)
 - [https://blogs.msdn.microsoft.com/webdev/2017/07/13/development-time-iis-support-for-asp-net-core-applications/](https://blogs.msdn.microsoft.com/webdev/2017/07/13/development-time-iis-support-for-asp-net-core-applications/)