# Package Handler Library Reference

The package handler project is one of the core projects.
It handles loading packages from the filesystem.
The built project needs to be installed into a startup directory or specified on execution to add it to the runtime.

> The Library Directory used to look up Packages is `data/subsystems/run/libs/`

Usage:
```js

//Import the Package Handler from the Runtime
let Package = Runtime.Import("Package");

//Import a Package by using the package handler.
//This will search for a supported "MyPackage" file in the "libs" directory.
let MyPackage = Package.Import("MyPackage");

```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)

[C# Documentation](/index.html)