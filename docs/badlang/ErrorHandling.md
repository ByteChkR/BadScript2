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

[C# Documentation](/index.html)