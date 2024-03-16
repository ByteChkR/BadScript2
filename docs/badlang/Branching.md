# Branching

Structures to execute code conditionally

## If Statement

Checks for a condition and executes the specified block if the condition evaluates to `true`.
```js
if(true)
{
	//This gets executed if the condition is true
}
```

If Branches can be daisychained with `else if`

```js
const a = 10;
if(a > 10)
{
	//A is greater than 10
}
else if(a == 10)
{
	//A is exactly 10
}
```

To Handle the case where none of the If Branches get executed, there can be an `else` statement at the end of all conditional branches.

```js
const a = 10;
if(a == 0)
{
	//A is exactly 0
}
else if(a > 10)
{
	//A is Greater than 10
}
else //None of the conditions above did match
{
	//A is not 10 and not greater than 10
}
```

## Switch Statements

Sometimes the code can be more expressive and easier to understand to write the conditions in a switch statement.

> All Cases that specify a code block need to either end with a `return` or a `break` statement.

> A case *MUST* specify the code to executed within a code block (`{}`).

```js

const language = "DE";
switch(language)
{
	case "EN":
	{
		return "Hello World!";
	}
	case "DE":
	{
		return "Hallo Welt!";
	}
}
```

Switch supports whats called a *fallthrough* behaviour.
This enables to reuse the same case blocks for multiple cases.
```js

const language = "DE";
switch(language)
{
	case "EN":
	{
		return "Hello World!";
	}
	case "AT": //If any of the cases is true, the next code block will be executed.
	case "CH":
	case "DE":
	{
		return "Hallo Welt!";
	}
}
```

If none of the cases match the input value, the default branch will be taken
```js

const language = "DE";
switch(language)
{
	case "AT": //If any of the cases is true, the next code block will be executed.
	case "CH":
	case "DE":
	{
		return "Hallo Welt!";
	}
	case "EN":
	default: //None of the cases above matched the input string. We default to english here.
	{
		return "Hello World!";
	}
}
```

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)