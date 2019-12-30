# StudentVue API
Unofficial .NET API for StudentVue Portals

This repository provides an easy way to access data from StudentVue portals in .NET programs.

## Dependencies
This library depends on HtmlAgilityPack and Newtonsoft.Json, which can be installed through the NuGet package manager:
```
Install-Package HtmlAgilityPack
Install-Package Newtonsoft.Json
```
Alternatively, these dependencies can also be found in the StudentVueAPI\Library folder.

## Logging In
Add the StudentVueAPI.dll reference in your project file and its required dependencies. These can be found in the StudentVueAPI\Library folder.

Create a new StudentVue object and call Login() with the username, password, and district domain as parameters.
```
using StudentVueAPI;

StudentVue studentVue = new StudentVue();
studentVue.Login("username", "password", "domain");
```
To check if you are successfully logged in, you can access the response uri of the HTTP response directly.
```
if (BrowserSession.ResponseUri == string.Format("https://{0}/Home_PXP2.aspx", "domain")) 
    Console.WriteLine("Successfully logged in.");
```
You are now free to use the StudentVue methods.

## Documentation
Documentation can be found within each .cs file and the .xml documentation file when importing the library into a project.

## Issues
Due to the nature of some school districts' StudentVue applications, there may be a session timeout in place. If the session expires, the StudentVue class will throw an error, and the user will have to log in again.

## TODO
- [ ] Write documentation
- [ ] Extend functionality
