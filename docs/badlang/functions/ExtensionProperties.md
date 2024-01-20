# Extension Properties

Functions have extension properties that can help with working with functions.

## `Name`-Property

To get the (original) name of the function, use the `Name` property.

```js
function F() {}
let unnamed = function() {}
let name = F.Name; //"F"
name = unnamed.Name; //"<anonymous>"
```

## `Parameters`-Property

To get an array of the parameters, use the `Parameters` property.
It returns an array of information objects for each parameter

```js
function F(x!, y?, z*) {}

let p = F.Parameters;

let xInfo = p[0];
/*
{
	Name: "x",
	IsNullChecked: true,
	IsOptional: false,
	IsRestArgs: false
}
*/

let yInfo = p[1];
/*
{
	Name: "y",
	IsNullChecked: false,
	IsOptional: true,
	IsRestArgs: false
}
*/
let zInfo = p[2];
/*
{
	Name: "z",
	IsNullChecked: false,
	IsOptional: false,
	IsRestArgs: true
}
*/


```

## `Invoke`-Property

A helper function that takes an argument array as input and invokes the function object with the parameters inside the array.

```js

function F(x!, y?, z*)
{
	//Code
}

F.Invoke([1, 2]); //x = 1, y = 2, z = []
F.Invoke([1, 2, 3, 4, 5]); //x = 1, y = 2, z = [3, 4, 5]

```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)