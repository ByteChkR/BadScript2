# BadScript2
Bad Script is an Interpreted Scripting Language written in pure C#. It has a similar syntax to javascript and written to be easily extensible.

This Repository contains a complete rewrite of [BadScript](https://github.com/ByteChkR/BadScript)

## Install + Hello World

Follow these steps to get started.

### Installing

Currently it is only possible to build the project from source. Binaries will be available on the first release.

#### Building from Source

Requirements:
- git
- net6.0 SDK
- Powershell

1. Clone this Repository `git clone https://github.com/ByteChkR/BadScript2`
2. Run `build.ps1` located inside the repository folder

`build.ps1` compiles the language project and builds all common libraries for the runtime.
The Compiled output will be generated in `./build`.

### Hello World
Create a file `helloworld.bs` with the following content:
```js
Console.WriteLine("Hello World!");
```

Run the script with the command
```
./path/to/bs.exe run -f helloworld.bs
```

Output:
```
Hello World!
```

## More Information

- [Getting Started](./GettingStarted.md)
- [Common API Reference](./common/api/Readme.md)
- [Common Extension Reference](./common/extensions/Readme.md)
- [Common Libraries Reference](./common/libraries/Readme.md)
- [Further Reading](./FurtherReading.md)