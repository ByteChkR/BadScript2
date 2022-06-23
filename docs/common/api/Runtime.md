# Runtime API Reference

The `Runtime` api has the following Properties:

```
{
        Evaluate: function Evaluate(src, file?, optimize?)
        GetStackTrace: function GetStackTrace()
        Native: {
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
        Compiler: {
                CompileFunction: function CompileFunction(BadExpressionFunction)
                CompileSource: function CompileSource(src, file?, optimize?)
        }
}
```
___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)