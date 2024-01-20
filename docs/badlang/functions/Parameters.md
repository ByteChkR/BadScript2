# Parameters

Functions can have named input parameters.

## Default

The Default input parameter consists of only a single word. The Name of the parameter.

```js
function F(x /*x is the input parameter*/)
{
	//Code
}

F(10); // X is 10 inside F
```

## NullChecked

Prepend `!` after the parameter name to disallow the passing of `null`-values to that parameter.

```js
function F(x! /*x is the input parameter*/)
{
	//Code
}

F(10); // X is 10 inside F
F(null); //Crashes execution. x does not allow null-values
```


## Optional

Prepend `?` after the parameter name to make the parameter optional.
Optional parameters can only have more optional parameters to their right.

```js
function F(x?)
{
	//Code
}

F(10); 	//X is 10 inside F
F(); 	// X is null inside F
		//Runtime automatically assigns null to the parameter if it is not specified
```

## Rest Args

Prepend `*` after the parameter name to let the runtime put all additional function arguments into a single list.
There can only be one `*` parameter in a function. It has to be the last parameter in the parameter list.

```js
function F(x*)
{
	//Code
}

F(); //x is an empty array
F(10, 20, 30); //x is an array of the input parameters [10, 20, 30]
```

## Mixing Parameter Modifiers

An optional nullchecked argument.
The parameter is only null if it has not been specified at all.

```js
function F(x!?) => x;
//OR
function F(x?!) => x;

F(10);
F();
F(null); //Crash
```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)