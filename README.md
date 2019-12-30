# StudentVue API
Unofficial .NET API for StudentVue Portals

This repository provides an easy way to access data from StudentVue portals in .NET programs.

## Dependencies
This library depends on HtmlAgilityPack and Newtonsoft.Json, which can be installed through the NuGet package manager:
```
Install-Package HtmlAgilityPack
Install-Package Newtonsoft.Json
```

## Logging In
Add the StudentVueAPI reference in your project file and its required dependencies. Don't forget to add the StudentVueAPI.xml documentation file

Create a new StudentVue object and call Login() with the username, password, and district domain as parameters.
```
using StudentVueAPI;

StudentVue studentVue = new StudentVue();
studentVue.Login("username", "password", "domain");
```

## Documentation
Documentation can be found within each .cs file and the .xml documentation file when importing into a project.

## TODO
- [ ] Write documentation
- [ ] Extend functionality
