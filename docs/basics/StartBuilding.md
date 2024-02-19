# Start Building

The Commandline comes with multiple BadScript apps preinstalled.
This page covers two of them.

___

## new

The `new` app is used to create new projects.

> Basic Usage: `bs new <template>`

Using this app is the recommended way to create new projects.

> Templates are stored in the directory returned from `bs settings Subsystems.New.TemplateDirectory`

___

## build

The `build` app is used to build and install a project.

> Basic Usage: `bs build [<target>]`

### Targets

The Build System has Multiple targets.

> To View all targets open the `build` directory located at the path that gets returned from `bs settings Console.SubsystemDirectory`

Most Common targets are `Debug` and `Release`.
Both targets will build and install the project into the runtime.

> The install location is one of the directories returned from `bs settings Subsystems.Run`

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)