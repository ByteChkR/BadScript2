# Operators

- [Comparison](#comparison)
	- [Equal(==)](#equal)
	- [NotEqual(!=)](#notequal)
	- [GreaterOrEqual(>=)](#greaterorequal)
	- [GreaterThan(>)](#greaterthan)
	- [LessOrEqual(<=)](#lessorequal)
	- [LessThan(<)](#lessthan)
- [Logic](#logic)
	- [And(&&)](#and)
	- [Or(\|\|)](#or)
	- [XOr(^)](#xor)
	- [Not(!)](#not)
	- [Assignment](#assignment)
		- [AndAssign(&=)](#andassign)
		- [OrAssign(\|=)](#orassign)
		- [XOrAssign(^=)](#xorassign)
- [Math](#math)
	- [Add(+)](#add)
	- [Subtract(-)](#subtract-)
	- [Multiply(\*)](#multiply)
	- [Divide(/)](#divide)
	- [Modulo(%)](#modulo)
	- [Exponentiation(\*\*)](#exponentiation)
	- [Assignment](#assignment_1)
		- [AddAssign(+=)](#addassign)
		- [SubtractAssign(-=)](#subtractassign-)
		- [MultiplyAssign(\*=)](#multiplyassign)
		- [DivideAssign(/=)](#divideassign)
		- [ModuloAssign(%=)](#moduloassign)
		- [ExponentiationAssign(\*\*=)](#exponentiationassign)
	- [Atomic](#atomic)
		- [PreIncrement(++)](#preincrement)
		- [PreDecrement(--)](#predecrement--)
		- [PostIncrement(++)](#postincrement)
		- [PostDecrement(--)](#postdecrement--)
- [Branching](#branching)
	- [NullCoalescing(??)](#nullcoalescing)
		- [NullCoalescingAssignment(??=)](#nullcoalescingassignment)
	- [Ternary(?)](#ternary)
- [Assignment(=)](#assignment_2)
- [Access](#access)
	- [MemberAccess(.)](#memberaccess)
		- [NullCheckedMemberAccess(.?)](#nullcheckedmemberaccess)
	- [ArrayAccess(\[\])](#arrayaccess)
		- [NullCheckedArrayAccess(?\[\])](#nullcheckedarrayaccess)
- [In](#in)
- [Instanceof](#instanceof)
- [Unary Unpack(...)](#unary-unpack)
- [Binary Unpack(...)](#binary-unpack)
___
___

## Comparison

All traditional binary comparison operators

___

### Equal(==)

Compares if two objects are __equal__

```js


function Run(n)
{
	if(n == null)//n is not equal to null
	{
		return false;
	}
	return true;
}

Run(true); //True
Run(false); //True
Run(null); //False

```

___

### NotEqual(!=)

Compares if two objects are __not equal__

```js

function Run(n)
{
	if(n != null)//n is not equal to null
	{
		return true;
	}
	return false;
}

Run(true); //True
Run(false); //True
Run(null); //False

```

___


### GreaterOrEqual(>=)

Compares if the left number is __greater or equal__ to the right number.

```js

function Run(n)
{

	if(n >= 100) //n is greater or equal to 100
	{
		
		return true;
	}

	return false;
}

Run(10); //False
Run(101); //True
Run(100); //True

```

___


### GreaterThan(>)

Compares if the left number is __greater than__ the right number.

```js

function Run(n)
{

	if(n > 100) //n is greater than 100
	{
		
		return true;
	}

	return false;
}

Run(10); //False
Run(101); //True
Run(100); //False

```

___

### LessOrEqual(>=)

Compares if the left number is __less or equal__ to the right number.

```js

function Run(n)
{

	if(n <= 100) //n is less or equal to 100
	{
		
		return true;
	}

	return false;
}

Run(10); //True
Run(100); //True
Run(101); //False

```

___

### LessThan(>)


Compares if the left number is __less than__ to the right number.

```js

function Run(n)
{

	if(n < 100) //n is less than to 100
	{
		
		return true;
	}

	return false;
}

Run(10); //True
Run(100); //False
Run(101); //False

```


___
___

## Logic

All traditional binary logic operators

___

### And(&&)


Returns true if the left __and__ the right side are true

```js

function Run(a, b)
{

	if(a && b) //a and b are true
	{
		
		return true;
	}

	return false;
}

Run(true, true); //True
Run(true, false); //False
Run(false, false); //False

```

___

### Or(||)


Returns true if the left __or__ the right side are true

```js

function Run(a, b)
{

	if(a || b) //a or b are true
	{
		
		return true;
	}

	return false;
}

Run(true, true); //True
Run(true, false); //True
Run(false, false); //False

```

___

### XOr(^)

Returns true if only the left __or__ only the right side are true

```js

function Run(a, b)
{

	if(a ^ b) //only a or only b are true
	{
		
		return true;
	}

	return false;
}

Run(true, true); //False
Run(true, false); //True
Run(false, false); //False

```

___

### Not(!)

Returns true if the input is negative

```js

function Run(a)
{

	if(!a) //a and b are true
	{
		
		return true;
	}

	return false;
}

Run(true); //False
Run(false); //True

```

___

### Assignment

Assigns the result of the operation to the left variable

___

#### AndAssign(&=)


Returns true if the left __and__ the right side are true.
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{

	a &= b; //a = a and b are true
	if(a)
	{
		return true;
	}

	return false;
}

Run(true, true); //True
Run(true, false); //False
Run(false, false); //False

```

___

#### OrAssign(|=)


Returns true if the left __or__ the right side are true.
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a |= b; //a = a or b are true
	if(a)
	{		
		return true;
	}

	return false;
}

Run(true, true); //True
Run(true, false); //True
Run(false, false); //False

```

___

#### XOrAssign(^=)


Returns true if only the left __or__ only the right side are true.
The resulting value will be stored in the left variable.


```js

function Run(a, b)
{

	a ^= b; //only a or only b are true
	if(a)
	{
		
		return true;
	}

	return false;
}

Run(true, true); //False
Run(true, false); //True
Run(false, false); //False

```

___
___
## Math

All traditional maths operations

___

### Add(+)


Adds two numbers or strings

```js

function Run(a, b)
{
	return a + b;
}

Run(1, 1); //2
Run("A", null); //Anull
Run("A", []); //A[]

```

___

### Subtract(-)


Subtracts two numbers

```js

function Run(a, b)
{
	return a - b;
}

Run(1, 1); //0

```

___

### Multiply(\*)

Multiplies two numbers

```js

function Run(a, b)
{
	return a * b;
}

Run(2, 2); //4

```

___

### Divide(/)

Divides two numbers

```js

function Run(a, b)
{
	return a / b;
}

Run(16, 2); //8

```

___

### Modulo(%)

Gets the remainder of the division

```js

function Run(a, b)
{
	return a % b;
}

Run(19, 10); //9
Run(10, 5); //0
Run(12, 3); //0

```

___

### Exponentiation(\*\*)

Gets the exponentiation of two numbers

```js

function Run(a, b)
{
	return a ** b;
}

Run(2, 2); //4
Run(2, 3); //8
Run(3, 2); //9

```

___

### Assignment

Assigns the result of the operation to the left variable

___

#### AddAssign(+=)


Adds two numbers or strings
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a += b;
	return a;
}

Run(1, 1); //2
Run("A", null); //Anull
Run("A", []); //A[]

```

___

#### SubtractAssign(-=)



Subtracts two numbers
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a -= b;
	return a;
}

Run(1, 1); //0

```

___


#### MultiplyAssign(\*=)


Multiplies two numbers
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a *= b;
	return a;
}

Run(2, 2); //4

```

___

#### DivideAssign(/=)

Divides two numbers
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a /= b;
}

Run(16, 2); //8

```

___

#### ModuloAssign(%=)

Gets the remainder of the division
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a %= b;
	return a;
}

Run(19, 10); //9
Run(10, 5); //0
Run(12, 3); //0

```

___

### ExponentiationAssign(\*\*=)

Gets the exponentiation of two numbers.
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a **= b;
	return a;
}

Run(2, 2); //4
Run(2, 3); //8
Run(3, 2); //9

```

___
___

### Atomic

Atomic Increment or Decrement

___

#### PreIncrement(++)

Increments the value by one and returns the new result
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = ++a; //2

```

___

#### PreDecrement(--)

Increments the value by one and returns the new result
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = --a; //0

```

___

#### PostIncrement(++)

Increments the value by one and returns the original value
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = a++; //1

```
___

#### PostDecrement(--)

Increments the value by one and returns the original value
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = a--; //1

```

___
___

## Branching

All traditional branching operators

___

### NullCoalescing(??)

Returns the left value if it is not equal to null. Otherwise return the right value

```js

function Run(a, b)
{
	return a ?? b;
}

Run(null, 10); //10
Run(10, 5); //10
Run(null, null); //null

```
___

#### NullCoalescingAssignment(??=)

Returns the left value if it is not equal to null. Otherwise return the right value.
The resulting value will be stored in the left variable.

```js

function Run(a, b)
{
	a ??= b;
	return a;
}

Run(null, 10); //10
Run(10, 5); //10
Run(null, null); //null

```
___

### Ternary(?)

Returns the left value if the condition is true. Otherwise return the right value.
The resulting value will be stored in the left variable.

```js

function Run(a, b, left)
{
	return left ? a : b;
}

Run(null, 10, false); //10
Run(10, 5, true); //10
Run("AAA", null, false); //null

```

___
___


## Assignment(=)

Assigns the value on the right to the variable on the left.

```js

let a = 10;

a = 100;

```

___
___

## Access

Operatiors related to accessing properties inside objects

___

### MemberAccess(.)

Selects a property from an object.

```js

let t = { MyValue: true }

t.MyValue; //true

t.MyValue = false;


```

___

#### NullCheckedMemberAccess(?.)

Selects a property from an object if the object is not null

```js

let t = { MyValue: true }

t.MyValue; //true

t = null;

t?.MyValue; //null


```

___

### ArrayAccess(\[\])

Selects a property from an object.

```js

let t = { MyValue: true }

t["MyValue"]; //true

t["MyValue"] = false;


```

___

#### NullCheckedArrayAccess(?\[\])

Selects a property from an object if the object is not null

```js

let t = { MyValue: true }

t["MyValue"]; //true

t = null;

t?["MyValue"]; //null


```

___

#### In

Returns true if a specified key exists in the specified object

```js

let t = { MyValue: "ABC" }

let exists = "MyValue" in t; //True

exists = "MyOtherValue" in t; //False

```

___

#### Instanceof

Returns true if a specified object implements the specified type or interface

```js

let number = 10;

let isNum = number instanceof num; //True

let isString = number instanceof string; //False

```

___

#### Unary Unpack(...)

Spreads the Content of the right side of the operator into the current scope

```js

let t = {MyValue: "abc"}


...t; // MyValue is now a variable in the current scope

const myVal = MyValue; //contains "abc"


```

#### Binary Unpack(...)

Spreads the Content of the right side of the operator into the object of the left side and returns the result.

```js

let a = {MyValue: "abc"}
let b = {MyNumber: 3.1415}

let combined = a...b;

const myVal = combined.MyValue; //contains "abc"
const myNum = combined.MyNumber; //contains 3.1415

```

___
___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)