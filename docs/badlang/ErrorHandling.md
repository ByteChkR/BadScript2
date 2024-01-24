# Error Handling

BadScript2 has error handling with exceptions.
You can throw exceptions
```js
throw "This is an error";
```

and you can catch exceptions
```js
function F() => throw "This is an error";
try
{
	F();
}
catch(e)
{
	Console.WriteLine(e); //Prints: "This is an error" + some runtime information such as file and line number.
}
```

## `finally`-Blocks

When using a `try`/`catch` block, it is possible to define a code block to run regardless of the result. This block is called the `finally`-Block

```js
function F() => throw "This is an error";
try
{
	F();
}
catch(e)
{
	Console.WriteLine(e); //Prints: "This is an error" + some runtime information such as file and line number.
}
finally
{
	Console.WriteLine("Ran the finally block!");
}
```

### The Error Object
Whenever an exception gets thrown. The runtime wraps the supplied object into an object
```js
{
	StackTrace: "", //StackTrace of where the error happened
	InnerError: {/*Error Object of inner exception*/},
	ErrorObject: "" //The actual object that is thrown by the user code
}
```

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)