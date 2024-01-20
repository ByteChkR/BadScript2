# Loops

Structures to repeat blocks of code.

## while

Repeats a block of code until the condition is true
```js
while(true) //Condition
{
	/*BLOCK*/
}
```

___

## for

Repeats a block of code until the condition is not met.

```js
for(let i = 0; i < 10; i++) //for(variable definition; condition; step;)
{

}
```

___

## foreach

Enumerates a collection like an array and runs the block of code for each element in the enumeration.

```js
let array = [1, 2, 3, 4, 5];

foreach(elem in array) //foreach(<name> in <enumerable>)
{
	//elem is an element contained in the array
}
```

___

> All Objects can be iterated on with a foreach method if they implement the following function

```js
function GetEnumerator() {} // returns an enumerator
```

> The `GetEnumerator` function should return an object that contains enumerator functions

```js
function GetCurrent() {} 	//Returns the current element

function MoveNext() {} 		//Moves the enumerator to the next element. 
							//Returning true if the next element exists. 
							//Returning false if the enumerator reached the end of the enumerable.
```

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)

[C# Documentation](/index.html)