# System Library Reference

The System Library is a collection of useful functions and structures.
```js
{
        Assert: {
                IsString: function IsString(obj)
                IsBoolean: function IsBoolean(obj)
                IsNumber: function IsNumber(obj)
                IsArray: function IsArray(obj)
                IsTable: function IsTable(obj)
                IsFunction: function IsFunction(obj)
                IsEnumerator: function IsEnumerator(obj)
                IsEnumerable: function IsEnumerable(obj)
        }
        Enumerables: {
                Range: function Range(from, to, step?)
                Repeat: function Repeat(obj, num)
                Infinite: function Infinite(obj)
        }
        Events: {
                Event: prototype Event
        }
        Logging: {
                Logger: prototype Logger
                TagLogger: prototype TagLogger
                CreateDefault: function CreateDefaultLogger()
        }
        SourceReader: prototype SourceReader
}
```


___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Common Libraries](./Readme.md)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)