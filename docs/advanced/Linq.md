# Using BadLinq

The Linq Extensions can be used to express the linq expressions in BadScript2 Syntax

```csharp
IEnumerable<int> e = Enumerable.Range(0, 123);
foreach (var v in e.Where("x => x % 2 == 0").Select("x => x * 2"))
{ 
    System.Console.WriteLine(v);
}
```
___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)

[C# Documentation](/index.html)