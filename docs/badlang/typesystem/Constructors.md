# Constructors

Classes defined in BadScript2 can have constructors that get executed when an instance of this class gets created.

```js

class C
{
	function C() //Constructor always has the same name as the class
	{
		Console.WriteLine("Hello from Constructor");
	}
}

let c = new C(); //Prints "Hello from Constructor"

```

## Constructors and Inheritance

Constructors of base classes can only be called explicitly.

```js

class C
{
	function C()
	{
		Console.WriteLine("Hello from Constructor C");
	}
}

class D : C
{
	function D()
	{
		base.C(); //Call base constructor (optional but recommended if it exists)
		Console.WriteLine("Hello from Constructor D");
	}
}

let d = new D();
//Prints "Hello from Constructor C"
//Prints "Hello from Constructor D"

```

> Constructors are public methods that can be called even after the object has been created.
> However it is strongly discouraged to do so.

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)