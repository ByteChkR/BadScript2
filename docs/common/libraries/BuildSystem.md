# BuildSystem Library Reference

The BuildSystem Library is the Library used to build BadScript Projects.

Script used to build BadScript Projects
> this script is located in `data/subsystems/run/apps/build.bs` when the runtime is built.
> To invoke the script call `bs build [targets]` from any folder.
```js

const Package = Runtime.Import("Package");

const BuildSystem = Package.Import("BuildSystem");


const args = Runtime.GetArguments();
const targets;
if(args.Length == 0)
{
        targets = ["default"];
}
else
{
        targets = args;
}

//Build all targets
foreach(target in targets)
{
        const bs = new BuildSystem();
        const context = {
                Builder: bs,
                DEFAULT_TARGETSCRIPT_PATH: IO.Path.Combine(IO.Directory.GetStartupDirectory(), "data", "subsystems", "build", "scripts"),
                DEFAULT_TARGET_PATH: IO.Path.Combine(IO.Directory.GetStartupDirectory(), "data", "subsystems", "build", "targets"),
                StartTarget: target
        }
        bs.AddTargetSource("./targets", context);
        bs.AddTargetSource(context.DEFAULT_TARGET_PATH, context);
        bs.Run(target, context);
}

```

## BuildSystem Paths
Build System Targets are searched in `./targets` in the current directory and in `./data/subsystems/build/targets` relative to the runtime path.

Build System Scripts are searched in `./scripts` in the current directory and in `./data/subsystems/build/scripts` relative to the runtime path.

## Build Targets
Example Build Target
```json
{
  "Name": "Build Target Name",
  "Description": "Target Description",
  "Dependencies": [
    "Targets",
    "That",
    "Run",
    "Before",
    "This",
    "Target"
  ],
  "Script": "$(DEFAULT_TARGETSCRIPT_PATH)/MYSCRIPT.bs"
}
```

If a target gets executed all dependencies get executed before the current target is executed.
Targets can provide a `Script` path that is invoked when the target gets executed and all dependencies completed successfully.

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Common Libraries](./Readme.md)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)