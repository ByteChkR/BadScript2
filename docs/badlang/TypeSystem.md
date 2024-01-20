# Type System

The BadScript2 Language supports the definition and usage of classes.

Classes are Special [Tables](NativeTypes.md#Table) that do not allow the assignment of previously undefined variables.

```js
class C
{
	let X;
}

let c = new C();
c.X = 10;
c.Y = 10; //Crash. Variable is not defined.

```

## Property Visibility

Classes can have `public`, `private` and `protected` members

The Visibility is specified by underscores `_`.

```js
class C
{
	let X; //Public
	let _Y; //Protected
	let __Z; //Private
}
```

### Interfaces

Interfaces are pseudo classes that if inherited, assert the interface shape at parse time when the program loads.

```js
	interface I
	{
		F(); //Requires a function F with no arguments to be present
	}

	class C : I //Implements the Interface
	{
		function F() //If this function is missing, the parsing fails.
		{
			return true;
		}
	}
```

#### Typed Interfaces

Interfaces can also specify the types of the properties.

```js
interface I
{
	bool F(num n!, bool compute?);
}
class C : I
{
	function F(n!, compute?)
	{
		if(compute == false) return true;
		return n == 10;
	}
}
```

#### Inheritance Chains

Interfaces can inherit from other interfaces, an implementing class needs to satisfy all constraints from all interfaces in the inheritance chain.

```js
interface IA
{
	bool F();
}
interface IB : IA
{
	bool G();
}
class C : IB //Implements IB which also requires IA.
{
	function bool F()
	{
		return true;
	}

	function bool G()
	{
		return F();
	}
}
```
___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)