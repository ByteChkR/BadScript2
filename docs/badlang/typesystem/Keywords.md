# Keywords

Inside Class Definitions there are 2 special keywords that can be used.

## this

Available in all classes. Contains a reference to itself.

> This is useful if a parameter of a function hides the member you are trying to access.

```js
class C
{
	let x;
	let y;

	function SetX(x)
	{
		//x = x; would assign the parameter to itself.
		this.x = x; //Assigns the parameter x to the property x of class C
	}
}
```

> Note: `this` is available even outside the class scope. It is strongly discuraged to use it from outside the class definition.

## base

Available in all classes that have a base class.
If a function hides a function inside the base class, the `base` keyword can be used to circumvent this issue.

```js

class C
{
	function Hello(v)
	{
		Console.WriteLine(v);
	}
}

class D : C //D inherits from C
{
	function Hello()
	{
		base.Hello("Hello World!"); //uses 'base' to get access to C.Hello(v) instead of D.Hello()
	}
}


```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)