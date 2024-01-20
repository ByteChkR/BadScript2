# Functions

Functions are named(or unnamed) procedures that can be executed multiple times.

## The basics

There are named functions
```js
function MyFunctionName()
{
	//Code
}
```

And unnamed functions
```js
function()
{
	//Code
}
```
> Unnamed functions can not be referenced after they are created. Its advised to assign them to a variable.

```js
let myFunc = function()
{
	//Code
}
```

### Single line blocks

Sometimes, functions only consist of one expression. For this usecase, single line blocks exist.

```js

function Double(n) => n * 2; //Single Line Block Named Function

let add = function(a, b) => a + b; //Single Line Block Unnamed Function

```

### Lambda Functions

For convenience the Single Line Functions can be expressed as lambda expressions.
Those functions can not define a return type.

```js

const Add = (a, b) => a + b; //Specifying Parameters with a parameter list.

const Double = x => x * 2; //Single Parameters can be specified without the brackets

const Double4 = () => Double(4); //If the function has no parameters the empty brackets must be specified.

```

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)