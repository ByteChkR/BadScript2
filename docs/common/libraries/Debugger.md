# Debugger Library Reference

The Debugger Project is one of the core projects that is used by the runtime to provide a scriptable debugger.

> The built project needs to be installed into `data/subsystems/run/Debugger.bs` to be properly picked up by the runtime.

To Debug a file specify the `-d/--debug` flag when executing a script.


> Debugging minified scripts is possible, but will lead to hard to read code.

## Commandline
The Debugger has a commandline.

### Useful Commands

#### `help`
Lists all available commands.

#### `step`
Toggles the step mode.

If the step mode is engaged, the debugger will halt at every line in the script.

#### `sbp`
Sets a breakpoint.

##### Arguments

```
sbp <-- Sets the breakpoint at the current line
sbp 2 <-- Sets the breakpoint at line 2 in the current file.
sbp MyFile.bs 2 <-- Sets the breakpoint at line 2 in the file "MyFile.bs"
sbp MyFile.bs 2 x == 10 <-- Sets the breakpoint at line 2 in file "MyFile.bs" with condition "x == 10"
```

#### `ubp`
Unsets a breakpoint.

##### Arguments
```
ubp MyFile.bs 2 <-- Unsets all breakpoints at line 2 in file "MyFile.bs"
```

#### `lbp`
Lists all breakpoints.

#### `view`
Prints the current position in the file.

#### `eval`
Evaluates the specified expression in context of the current scope in the debugged file.

##### Arguments
```
eval x <-- Evaluates the variable 'x' at the current debug step.
```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)

[C# Documentation](/index.html)