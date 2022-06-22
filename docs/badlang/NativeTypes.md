# Native Types

BadScript2 comes with base types that are implemented in the host language(C#)

## bool

Contains a Boolean Value. Either `true` or `false`

```js
let v = true;
```


## num

Contains a Numeric Value.

```js
let v = 0.46;
```

## string

Contains Immutable String Literals.

```js
let v = "Hello";
```

## Function

Contains an Executable set of expressions.

```js
//Default way of defining a function
function F()
{
	return "Hello"
}

//Single line functions
function G() => "Hello";

//Function declared without inlined variable name
let H = function() {
	return "Hello";
}

//Single line function declared without inlined variable name
let J = function() => "Hello";
```

## Array

Contains a list of values.

```js
let a = []; //Empty Array
let b = ["Hello", "World"]; //Array with initial values.

let hello = b[0]; //Select First element of the array
let world = b[1]; //Select Second element of the array

b[0] = "Bye"; //Replace the first element of the array with another value
```

## Table

Contains a list of key value pairs.

```js

let t = {} //Empty Table
let v = {Key: "Value"} //Table with initial values

let val1 = v.Value; //Access element with key "Key"
let val2 = v["Key"]; //Access element with key "Key"

v.Key = "Val"; //Assign element at key "Key"
v["Key"] = "Val"; //Assign element at key "Key"

```


___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)