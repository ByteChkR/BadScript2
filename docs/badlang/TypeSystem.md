# Type System

The BadScript2 Language supports the definition and usage of classes.

Classes are Special [Tables](NativeTypes.md#Table) that do not allow the assignment of previously undefined variables.

```js
class C
{
	let X;
}

let c = new C();
c.X = 10;
c.Y = 10; //Crash. Variable is not defined.

```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)