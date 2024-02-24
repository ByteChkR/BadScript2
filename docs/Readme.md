[![build](https://github.com/ByteChkR/BadScript2/actions/workflows/dotnet.yml/badge.svg)](https://github.com/ByteChkR/BadScript2/actions/workflows/dotnet.yml)

# BadScript2 Landing Page

<div style="display: flex; justify-content: center;">
<img width="256" height="256" src="https://github.com/ByteChkR/BadScript2/blob/master/res/Logo.png?raw=true"/>
</div>

Bad Script is an Interpreted Scripting Language written in pure C#. It has a similar syntax to javascript and written to be easily extensible.

This Repository contains a complete rewrite of [BadScript](https://github.com/ByteChkR/BadScript)

## Preparations for the public branch

At this point in time, there is a single dependency that is not available from the public nuget.org repository.
To be able to restore the solution, the project `BadScript2.IO.OpenKM` needs to be unloaded.

## Install + Hello World

Follow these steps to get started.

### Installing

Currently it is only possible to build the project from source. Binaries will be available on the first release.

#### Building from Source

Tested on:
- Windows 10 64-bit
- Debian 11 (Bullseye)

Requirements:
- git
- net6.0 SDK
- Powershell 7.0 or greater

1. Open a Powershell Session
	- On Windows: `powershell`
	- On Linux: `pwsh`
2. Clone this Repository
3. Change Directory to `./BadScript2`
3. Run `./build.ps1`

> use `build.ps1 -config Release` to build with optimizations.

> use `build.ps1 -writeLog` to get debug logs

`build.ps1` compiles the language project and builds all common libraries for the runtime.
The Compiled output will be generated in `./build`.


### Hello World
Create a file `helloworld.bs` with the following content:
```js
Console.WriteLine("Hello World!");
```

Run the script with the command
```
bs run -f helloworld.bs
```

Output:
```
Hello World!
```

## More Information

- [Getting Started](./GettingStarted.md)
- [Common Extension Reference](./common/extensions/Readme.md)
- Common Libraries Reference(coming soon)
- [Further Reading](./FurtherReading.md)


___

## Links

[Home](./Readme.md)

[Getting Started](./GettingStarted.md)