# Json API Reference

The `Json` api has the following Properties to convert Json Strings to BadObjects and back.
Also the API exposes readonly access to the runtime configuration.

```
{
        FromJson: <interop> function FromJson(str!)
        ToJson: <interop> function ToJson(BadObject)
        Settings: {
          Console: {
                  RootDirectory: %RUNTIME_DIR%
                  DataDirectory: %RUNTIME_DIR%/data
                  SubsystemDirectory: %RUNTIME_DIR%/data/subsystems
                  SettingsDirectory: %RUNTIME_DIR%/data/settings
                }

          Logging: {
                  LogForegroundColor: Blue
                  LogBackgroundColor: Black
                  WarnForegroundColor: DarkYellow
                  WarnBackgroundColor: Black
                  ErrorForegroundColor: Red
                  ErrorBackgroundColor: Black
                  Writer: {
                          Mask: [
                                  "Settings",
                                  "Runtime",
                                  "Benchmark",
                                  "Debugger",
                                  "Compiler"
                                ]
                        }

                }

          Runtime: {
                  Debugger: {
                          Path: %RUNTIME_DIR%/data/subsystems/run/Debugger.bs
                        }

                  FileExtension: bs
                  NativeOptimizations: {
                          UseStringCaching: True
                          UseConstantExpressionOptimization: True
                          UseStaticExtensionCaching: True
                          UseConstantFunctionCaching: True
                        }

                  Task: {
                          TaskIterationTime: 1
                        }

                }

          Subsystems: {
                  Html: {
                          RootDirectory: %RUNTIME_DIR%/data/subsystems/html
                          ExtensionDirectory: %RUNTIME_DIR%/data/subsystems/html/extensions
                        }

                  Run: {
                          RootDirectory: %RUNTIME_DIR%/data/subsystems/run
                          LibraryDirectory: %RUNTIME_DIR%/data/subsystems/run/libs
                          StartupDirectory: %RUNTIME_DIR%/data/subsystems/run/startup
                        }

                  Test: {
                          RootDirectory: %RUNTIME_DIR%/data/subsystems/test
                          TestDirectory: %RUNTIME_DIR%/data/subsystems/test/tests
                        }

                }

        }
}
```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)