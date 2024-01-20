# Variables

Variables can be defined in any scope.

Syntax:
```js
let [<type>] <varname>;
const [<type>] <varname>;
```

___

Variables can only be defined once. Defining the same variable again will result in an error.

```js
let v;

let v; //Crash
```

___

A Variable can be defined with an optional typename.

```js
let string v;
```

Only values with the same type or one of its subtypes can be assigned to this variable.

Note: `null` can be assigned to all types.

___


Variables can be declared as constants with the `const` keyword.
Those variables can not be assigned to after setting its initial value.

```js
const v;
//v is null
```

```js
const v;
//v is null
v = 10;
//v is 10
v = 11; //Crash. Variable is constant
````

___

Two Variables with the same name can exist, but not in the same scope as the current one.

```js
let v = 10;

function F()
{
	let v = 11;
	//v is 11
}
```

The Definition closest to the usage is hiding the other definitions.

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)

[C# Documentation](/index.html)