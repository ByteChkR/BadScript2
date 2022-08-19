# Project Library Reference

The Project Library is a core project. It provides functions and procedures that aid in building a script from multiple files.

## Usage

Minimal Example
```js
//Load Package Library
let Package = Runtime.Import("Package");

//Load Project Builder & Utils
let Project = Package.Import("Project");
let ProjectUtils = Package.Import("ProjectUtils");


//Create a Project Instance
let project = new Project();

project
	.AddSetting("BuildInfo", {OutputFile: "./bin/Program.bs"}) //Add the Build Info used to write the output
	.AddSource("path/to/source.bs") //Add source files
	.AddSource("path/to/folder/*.bs") //Add folders containing source files
	.AddPostprocessTask( //Add the Post process task "SaveToOutput"
							"Save Output", 
							ProjectUtils.SaveToOutput(project)
						)
	.Run(); //Run the Project Build
	//The output will contain the combined sources written to a single file.
```

> To get more examples look into the `/projects/<project>/build.bs` files.



___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Common Libraries](./Readme.md)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)