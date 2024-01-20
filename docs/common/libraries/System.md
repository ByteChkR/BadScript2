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
                Primes: function Primes()
        }
        Events: {
                Event: class Event
        }
        Logging: {
                Logger: class Logger
                TagLogger: class TagLogger
                CreateDefault: function CreateDefaultLogger()
        }
        SourceReader: class SourceReader
        Math: {
                IsPrime: function IsPrime(n)
                Sum: function Sum(n)
        }
}
```


___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)