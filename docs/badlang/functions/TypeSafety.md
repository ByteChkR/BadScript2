# Type Safety

Functions in BadScript2 support parameter type safety.
It allows the runtime to check the types of the parameters prior to running the function code.

```js

function F(num i)
{
	//Code
}

F(10); //Works. 10 is a number
F(null); //Works. null does not have type constraints
F(""); //Crash. string != num

```

> To disallow `null`, add the `!` parameter modifier to the desired parameter.

```js

function F(num i!)
{
	//Code
}

F(10); //Works. 10 is a number
F(null); //Crash. ! modifier does not allow null values.
F(""); //Crash. string != num

```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)

[C# Documentation](/index.html)