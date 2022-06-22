# Commandline

The Commandline Application included in this Project has multiple subsystems.

## Subsystem `run`

The `run` subsystem can run BadScripts inside a commandline environment.

Example Commands:
```
bs.exe run -f <inputfile>
bs.exe run -f <inputfile> -a <argument>
```

Commandline Flags:
```
  -f, --files          The files to run.

  -i, --interactive    Run in interactive mode.

  -a, --args           Arguments to pass to the script.

  -b, --benchmark      Set flag to Measure Execution Time.

  -d, --debug          Set flag to Attach a Debugger.

  --help               Display this help screen.

  --version            Display version information.
```

## Subsystem `test`

The `test` subsystem can run BadScript files inside a Unit Test Environment using the NUnit Framework.

Example Commands:
```
bs.exe test
```

Commandline Flags:
```
  --help               Display this help screen.

  --version            Display version information.
```


## Subsystem `settings`

The `settings` subsystem can load configuration files from the internal settings.

Example Commands:
```
bs.exe settings <settingspath>
```

Commandline Flags:
```
  --help          Display this help screen.

  --version       Display version information.
```


___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)