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

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)