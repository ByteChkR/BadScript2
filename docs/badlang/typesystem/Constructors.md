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
		base(); //Call base constructor (optional but recommended if it exists)
		Console.WriteLine("Hello from Constructor D");
	}
}

let d = new D();
//Prints "Hello from Constructor C"
//Prints "Hello from Constructor D"

```

## Primary Constructors

Primary Constructors aim to make it easier to create Immutable Data Types.

Instead of explicitly declaring the Properties and the Constructor, it can be done in a oneliner.

The Original way of declaring an immutable 2D Point would look like this.
```js

//Declares a point that has two immutable properties.
class Point
{
	const num X;
	const num Y;
	function Point(num x!, num y!)
	{
		X = x;
		Y = y;
	}
}

```

But it can also be declared with a primary constructor.

```js

//Declares a point that has two immutable properties.
//	Primary Constructor Implicitly declares two constant number fields X and Y. and assigns the values in the constructor.
class Point(num X!, num Y!);

```

### Primary Constructors with inheritance

Primary constructors can also be used to pass arguments to the base constructor.

> If a class declared with a primary constructor is calling a base constructor within the inheritance list(see below), its unable to declare any properties itself.
> Any unused parameters in the Primary constructor will be ignored.

```js

class Result(bool Success!, string message?);

class Success(string? message) : Result(true, message);

class Error(string? message) : Result(false, message);

```

### Primary Constructors with Custom Methods

Sometimes its useful to implement custom functions within a class declared with a primary constructor.

To illustrate with a 2D Point Class:

```js

class Point(num X!, num Y!)
{ 
	//Declare a class body like normal

	//Declare a function, using the implcitly declared properties.
	//	(keep in mind that those properties are immutable)
	function Point Add(Point other!)
	{
		return new Point(X + other.X, Y + other.Y);
	}

	//Operator Override that allows adding two points with +
	function Point op_Add(Point other!)
	{
		return Add(other);
	}

	//Helper function that returns the origin
	static function Point Origin() => new Point(0, 0);

	function string ToString()
	{
		return $"X: {X}, Y: {Y}";
	}
}

const Point a = Point.Origin();
const Point b = new Point(2, 2);
const Point c = new Point(1, 1);

Console.WriteLine(a + b + c); //Add all 3 points together
//Prints: X: 3, Y: 3

```

> This also works with inheritance!

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)