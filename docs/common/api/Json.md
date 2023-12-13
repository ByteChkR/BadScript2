# Json API Reference

The `Json` api has the following Properties to convert Json Strings to BadObjects and back.
Also the API exposes readonly access to the runtime configuration.

```
{
        Info: {
                Name: Json
                Version: 2023.10.8.0
                AssemblyName: BadScript2.Interop.Json
        }
        FromJson: <interop> function FromJson(str!)
        ToJson: <interop> function ToJson(BadObject!)
        Settings: {
          Logging: {
                  Writer: {
                          Mask: [
                                  "None"
                                ]
                        }

                  LogForegroundColor: Blue
                  LogBackgroundColor: Black
                  WarnForegroundColor: DarkYellow
                  WarnBackgroundColor: Black
                  ErrorForegroundColor: Red
                  ErrorBackgroundColor: Black
                }

          Console: {
                  RootDirectory: <tooldir>
                  DataDirectory: <tooldir>\data
                  SubsystemDirectory: <tooldir>\data/subsystems
                  SettingsDirectory: <tooldir>\data/settings
                }

          Runtime: {
                  Debugger: {
                          Path: <tooldir>\data/subsystems/run/Debugger.bs
                        }

                  FileExtension: bs
                  NativeOptimizations: {
                          UseStringCaching: True
                          UseConstantFoldingOptimization: True
                          UseConstantSubstitutionOptimization: True
                          UseStaticExtensionCaching: True
                          UseConstantFunctionCaching: True
                        }

                  Task: {
                          TaskIterationTime: 1
                        }

                  CatchRuntimeExceptions: True
                }

          Subsystems: {
                  Run: {
                          RootDirectory: <tooldir>\data/subsystems/run
                          LibraryDirectory: <tooldir>\data/subsystems/run/libs
                          StartupDirectory: <tooldir>\data/subsystems/run/startup
                          AppDirectory: <tooldir>\data/subsystems/run/apps
                          AppDataDirectory: <tooldir>\data/subsystems/run/appdata
                        }

                  Test: {
                          RootDirectory: <tooldir>\data/subsystems/test
                          TestDirectory: <tooldir>\data/subsystems/test
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