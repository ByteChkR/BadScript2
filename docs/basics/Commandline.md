# Commandline

The Commandline Application included in this Project has multiple subsystems.

## Subsystem `docs`

The `docs` subsystem can be used to display a auto generated documentation of all apis.

Example Command:
```
bs docs
```

## Subsystem `run`

The `run` subsystem can run BadScripts inside a commandline environment.

Example Commands:
```
bs run -f <inputfile>
bs run -f <inputfile> -a <argument>
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
bs test
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
bs settings <settingspath>
```

Commandline Flags:
```
  --help          Display this help screen.

  --version       Display version information.
```

## Subsystem `remote`

The `remote` subsystem can host or connect to a remote console on another machine.

Example Commands:
```
bs remote localhost 1337
```

Commandline Arguments:
```
  --help          Display this help screen.

  --version       Display version information.

  value pos. 0    Required. (Default: localhost) The Host to connect to

  value pos. 1    Required. (Default: 1337) The Host port to connect to
```

## Subsystem `html`

The `html` subsystem exposes the BadHtml Template Engine.

Example Commands:
```
bs html <inputfile>
bs html -f <inputfiles>
```

Commandline Arguments:
```
  -f, --files         The files to run.

  --model             The Model that the templates will use

  -d, --debug         Set flag to Attach a Debugger.

  -r, --remote        Specifies the Remote Console Host port. If not specified the remote host will not be started

  --skipEmptyNodes    If enabled, empty text nodes will be skipped.

  -m, --minify        If enabled, the output will be minified.

  --help              Display this help screen.

  --version           Display version information.
```


___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)