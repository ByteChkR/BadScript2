# Inheritance

Classes defined in BadScript2 can be inherited from.
All properties of the base class are available in the inheriting class.


## Checking inheritance

A class instance can be checked if it inherits from a specific base class.

```js
class B {}
class C {}
class D : C {}

let d = new D();

d.IsInstanceOf(B); // False
d.IsInstanceOf(C); // True
d.IsInstanceOf(D); // True

B.IsAssignableFrom(d); //False
C.IsAssignableFrom(d); //True
D.IsAssignableFrom(d); //True

B.IsBaseClassOf(D); //False
C.IsBaseClassOf(D); //True
D.IsBaseClassOf(D); //True

B.IsSuperClassOf(D); //False
C.IsSuperClassOf(D); //False
D.IsSuperClassOf(D); //True
D.IsSuperClassOf(C); //True

```

## Inheritance with `this`

If the `this` keyword is use inside a class that is inherited from, the value of `this` is the highest super class in the inheritance chain.

```js

class C 
{
	function WriteThis()
	{
		Console.WriteLine(this);
	}
}

class D : C {}

let c = new C();
c.WriteThis(); //Prints class C

let d = new D();
d.WriteThis(); //Prints class D

```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)