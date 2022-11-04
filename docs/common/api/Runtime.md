# Runtime API Reference

The `Runtime` api has the following Properties:

```
{
        Evaluate: function Evaluate(src, file?, optimize?, scope?)
        EvaluateAsync: function EvaluateAsync(src, file?, optimize?, scope?)
        CreateDefaultScope: function CreateDefaultScope()
        GetStackTrace: function GetStackTrace()
        Native: {
                ParseNumber: function ParseNumber(String)
                IsNative: function IsNative(BadObject)
                IsFunction: function IsFunction(BadObject)
                IsTable: function IsTable(BadObject)
                IsString: function IsString(BadObject)
                IsNumber: function IsNumber(BadObject)
                IsBoolean: function IsBoolean(BadObject)
                IsArray: function IsArray(BadObject)
                IsEnumerable: function IsEnumerable(BadObject)
                IsEnumerator: function IsEnumerator(BadObject)
        }
        Export: function Export(String, BadObject)
        Import: function Import(String)
        HasPackage: function HasPackage(String)
        GetArguments: function GetArguments()
        GetExtensionNames: function GetExtensionNames(BadObject)
        GetGlobalExtensionNames: function GetGlobalExtensionNames()
        GetTimeNow: function GetTimeNow()
}
```

> `Runtime.Evaluate` is executing a script fully synchronously. This can lead to problems with the `await` keyword and the `BadTask` abstraction. It is advised to use `Runtime.EvaluateAsync` instead.

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)