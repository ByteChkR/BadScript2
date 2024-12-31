# BadScript2 Language Documentation

Language documentation for BadScript2

## Table of Contents

- [BadScript2 Console Documentation](#badscript2-console-documentation)
    - [Table of Contents](#table-of-contents)
    - [Variables](#variables)
    - [Native Types](#native-types)
        - [bool](#bool)
        - [num](#num)
        - [string](#string)
        - [Function](#function)
        - [Array](#array)
        - [Table](#table)
    - [Operators](#operators)
        - [Comparison](#comparison)
            - [Equal(==)](#equal)
            - [NotEqual(!=)](#notequal)
            - [GreaterOrEqual(>=)](#greaterorequal)
            - [GreaterThan(>)](#greaterthan)
            - [LessOrEqual(>=)](#lessorequal)
            - [LessThan(>)](#lessthan)
        - [Logic](#logic)
            - [And(&&)](#and)
            - [Or(||)](#or)
            - [XOr(^)](#xor)
            - [Not(!)](#not)
            - [Assignment](#assignment)
                - [AndAssign(&=)](#andassign)
                - [OrAssign(|=)](#orassign)
                - [XOrAssign(^=)](#xorassign)
        - [Math](#math)
            - [Add(+)](#add)
            - [Subtract(-)](#subtract)
            - [Multiply(*)](#multiply)
            - [Divide(/)](#divide)
            - [Modulo(%) ](#modulo)
            - [Exponentiation(**)](#exponentiation)
            - [Assignment](#assignment-1)
                - [AddAssign(+=)](#addassign)
                - [SubtractAssign(-=)](#subtractassign)
                - [MultiplyAssign(*=)](#multiplyassign)
                - [DivideAssign(/=)](#divideassign)
                - [ModuloAssign(%=)](#moduloassign)
                - [ExponentiationAssign(**=)](#exponentiationassign)
            - [Atomic](#atomic)
                - [PreIncrement(++)](#preincrement)
                - [PreDecrement(--)](#predecrement)
                - [PostIncrement(++)](#postincrement)
                - [PostDecrement(--)](#postdecrement)
        - [Branching](#branching)
                - [NullCoalescing(??)](#nullcoalescing)
                - [NullCoalescingAssignment(??=)](#nullcoalescingassignment)
                - [Ternary(?)](#ternary)
        - [Assignment(=)](#assignment)
        - [Access](#access)
                - [MemberAccess(.)](#memberaccess)
                - [NullCheckedMemberAccess(?.)](#nullcheckedmemberaccess)
                - [ArrayAccess([])](#arrayaccess)
                - [NullCheckedArrayAccess(?[].)](#nullcheckedarrayaccess)
                - [In](#in)
                - [Instanceof](#instanceof)
                - [Unary Unpack(...)](#unary-unpack)
                - [Binary Unpack(...)](#binary-unpack)
                - [Range Operator(a..b)](#range-operatorab)
    - [Comments](#comments)
    - [Branching](#branching-1)
        - [If Statement](#if-statement)
        - [Switch Statements](#switch-statements)
    - [Loops](#loops)
        - [while](#while)
        - [for](#for)
        - [foreach](#foreach)
    - [Locking](#locking)
    - [Using](#using)
    - [Modules](#modules)
        - [Importing Modules](#importing-modules)
            - [Importing Installed Modules](#importing-installed-modules)
            - [Building Modules](#building-modules)
        - [Import Resolution](#import-resolution)
    - [Functions](#functions)
        - [The basics](#the-basics)
            - [Single line blocks](#single-line-blocks)
            - [Lambda Functions](#lambda-functions)
        - [TypeSafety](#type-safety)
        - [Parameters](#parameters)
            - [Default](#default)
            - [NullChecked](#nullchecked)
            - [Optional](#optional)
            - [Rest Args](#rest-args)
            - [Mixing Parameter Modifiers](#mixing-parameter-modifiers)
        - [Extension Properties](#extension-properties)
            - [`Name`-Property](#name-property)
            - [`Parameters`-Property](#parameters-property)
            - [`Invoke`-Property](#invoke-property)
    - [TypeSystem](#typesystem)
        - [Property Visibility](#property-visibility)
        - [Interfaces](#interfaces)
            - [Typed Interfaces](#typed-interfaces)
        - [Inheritance Chains](#inheritance-chains)
        - [Checking Inheritance](#checking-inheritance)
        - [Inheritance with `this`](#inheritance-with-this)
        - [Member Visibility](#member-visibility)
    - [Keywords](#keywords)
        - [this](#this)
        - [base](#base)
    - [Operator Overrides](#operator-overrides)
        - [Array Access(op_ArrayAccess)](#array-accessop_arrayaccess)
        - [Invocation(op_Invoke)](#invocationop_invoke)
        - [Math Assignment(op_AddAssign/op_SubtractAssign/...)](#math-assignmentop_addassignop_subtractassign)
    - [Error Handling](#error-handling)
        - [`finally`-Blocks](#finally-blocks)
        - [The Error Object](#the-error-object)
    - [Optimizing Tips](#optimizing-tips)
        - [Constant Function Optimization](#constant-function-optimization)
            - [Example](#example)
        - [Compiling Functions](#compiling-functions)
            - [Runtime Compilation](#runtime-compilation)
            - [Enable fast execution of compiled functions](#enable-fast-execution-of-compiled-functions)
        - [Combining Optimizations](#combining-optimizations)
        - [Benchmark Results for `Sum`](#benchmark-results-for-sum)

## Variables

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

## Native Types

BadScript2 comes with base types that are implemented in the host language(C#)

### bool

Contains a Boolean Value. Either `true` or `false`

```js
let v = true;
```


### num

Contains a Numeric Value.

```js
let v = 0.46;
```

### string

Contains Immutable String Literals.

```js
let v = "Hello";
```

### Function

Contains an Executable set of expressions.

Read [Functions](./Functions.md) for more information.

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

### Array

Contains a list of values.

```js
let a = []; //Empty Array
let b = ["Hello", "World"]; //Array with initial values.

let hello = b[0]; //Select First element of the array
let world = b[1]; //Select Second element of the array

b[0] = "Bye"; //Replace the first element of the array with another value
```

### Table

Contains a list of key value pairs.

```js

let t = {} //Empty Table
let v = {Key: "Value"} //Table with initial values

let val1 = v.Value; //Access element with key "Key"
let val2 = v["Key"]; //Access element with key "Key"

v.Key = "Val"; //Assign element at key "Key"
v["Key"] = "Val"; //Assign element at key "Key"

```

## Operators

### Comparison

All traditional binary comparison operators

___

#### Equal(==)

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

#### NotEqual(!=)

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


#### GreaterOrEqual(>=)

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


#### GreaterThan(>)

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

#### LessOrEqual(>=)

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

#### LessThan(>)


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

### Logic

All traditional binary logic operators

___

#### And(&&)


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

#### Or(||)


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

#### XOr(^)

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

#### Not(!)

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

#### Assignment

Assigns the result of the operation to the left variable

___

##### AndAssign(&=)


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

##### OrAssign(|=)


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

##### XOrAssign(^=)


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

#### Add(+)


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

#### Subtract(-)


Subtracts two numbers

```js

function Run(a, b)
{
	return a - b;
}

Run(1, 1); //0

```

___

#### Multiply(\*)

Multiplies two numbers

```js

function Run(a, b)
{
	return a * b;
}

Run(2, 2); //4

```

___

#### Divide(/)

Divides two numbers

```js

function Run(a, b)
{
	return a / b;
}

Run(16, 2); //8

```

___

#### Modulo(%)

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

#### Exponentiation(\*\*)

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

#### Assignment

Assigns the result of the operation to the left variable

___

##### AddAssign(+=)


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

##### SubtractAssign(-=)



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


##### MultiplyAssign(\*=)


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

##### DivideAssign(/=)

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

##### ModuloAssign(%=)

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

#### ExponentiationAssign(\*\*=)

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

#### Atomic

Atomic Increment or Decrement

___

##### PreIncrement(++)

Increments the value by one and returns the new result
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = ++a; //2

```

___

##### PreDecrement(--)

Increments the value by one and returns the new result
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = --a; //0

```

___

##### PostIncrement(++)

Increments the value by one and returns the original value
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = a++; //1

```
___

##### PostDecrement(--)

Increments the value by one and returns the original value
The resulting value will be stored in the left variable.

```js

let a = 1;
let b = a--; //1

```

___
___

### Branching

All traditional branching operators

___

#### NullCoalescing(??)

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

##### NullCoalescingAssignment(??=)

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

#### Ternary(?)

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


### Assignment(=)

Assigns the value on the right to the variable on the left.

```js

let a = 10;

a = 100;

```

___
___

### Access

Operatiors related to accessing properties inside objects

___

#### MemberAccess(.)

Selects a property from an object.

```js

let t = { MyValue: true }

t.MyValue; //true

t.MyValue = false;


```

___

##### NullCheckedMemberAccess(?.)

Selects a property from an object if the object is not null

```js

let t = { MyValue: true }

t.MyValue; //true

t = null;

t?.MyValue; //null


```

___

#### ArrayAccess(\[\])

Selects a property from an object or array.

```js

let t = { MyValue: true }

t["MyValue"]; //true

t["MyValue"] = false;


```

##### Slicing Array Access

It is possible to slice an array or table by supplying the Array Access Expression with a list of Keys or Indices.

```js

let t = {MyFirstValue: true, MySecondValue: false, MyThirdValue: true};

//Create a new table with only the selected keys:
let selected = ["MyFirstValue", "MyThirdValue"];
let result = t[selected];

//Result contains: {MyFirstValue: true, MyThirdValue: true}

```

> For arrays this works the same way, but instead it requires the indices instead of the keys.
> Using the [Range Operator](#range-operatorab) is a very clean way to get only a subset of an array.

___

##### NullCheckedArrayAccess(?\[\])

Selects a property from an object if the object is not null

```js

let t = { MyValue: true }

t["MyValue"]; //true

t = null;

t?["MyValue"]; //null


```

___

##### In

Returns true if a specified key exists in the specified object

```js

let t = { MyValue: "ABC" }

let exists = "MyValue" in t; //True

exists = "MyOtherValue" in t; //False

```

___

##### Instanceof

Returns true if a specified object implements the specified type or interface

```js

let number = 10;

let isNum = number instanceof num; //True

let isString = number instanceof string; //False

```

___

##### Unary Unpack(...)

Spreads the Content of the right side of the operator into the current scope

```js

let t = {MyValue: "abc"}


...t; // MyValue is now a variable in the current scope

const myVal = MyValue; //contains "abc"


```

##### Binary Unpack(...)

Spreads the Content of the right side of the operator into the object of the left side and returns the result.

```js

let a = {MyValue: "abc"}
let b = {MyNumber: 3.1415}

let combined = a...b;

const myVal = combined.MyValue; //contains "abc"
const myNum = combined.MyNumber; //contains 3.1415

```

##### Range Operator(a..b)

Creates an enumeration that contains a to (exclusive) b

```js

let even = [];
foreach(n in 0..100) //Gets the first 100 even numbers
{
	even.Add(n * 2);
}

```

> Particularly useful for [Slicing with the Array Access Operator](#slicing-array-access)


## Comments

BadScript2 supports single line comments as well as multi line comments

Single Line Comments Example:
```js
Console.WriteLine("Hello World!"); //This is a single line comment
function F( x //Comments can be in every position and between any token
)
{

}
```

Multi Line Comments Example:
```js
/*
	Everything inside here is a comment
	This
	and this
	and this
and this*/
```


## Branching

Structures to execute code conditionally

### If Statement

Checks for a condition and executes the specified block if the condition evaluates to `true`.
```js
if(true)
{
	//This gets executed if the condition is true
}
```

If Branches can be daisychained with `else if`

```js
const a = 10;
if(a > 10)
{
	//A is greater than 10
}
else if(a == 10)
{
	//A is exactly 10
}
```

To Handle the case where none of the If Branches get executed, there can be an `else` statement at the end of all conditional branches.

```js
const a = 10;
if(a == 0)
{
	//A is exactly 0
}
else if(a > 10)
{
	//A is Greater than 10
}
else //None of the conditions above did match
{
	//A is not 10 and not greater than 10
}
```

### Switch Statements

Sometimes the code can be more expressive and easier to understand to write the conditions in a switch statement.

> All Cases that specify a code block need to either end with a `return` or a `break` statement.

> A case *MUST* specify the code to executed within a code block (`{}`).

```js

const language = "DE";
switch(language)
{
	case "EN":
	{
		return "Hello World!";
	}
	case "DE":
	{
		return "Hallo Welt!";
	}
}
```

Switch supports whats called a *fallthrough* behaviour.
This enables to reuse the same case blocks for multiple cases.
```js

const language = "DE";
switch(language)
{
	case "EN":
	{
		return "Hello World!";
	}
	case "AT": //If any of the cases is true, the next code block will be executed.
	case "CH":
	case "DE":
	{
		return "Hallo Welt!";
	}
}
```

If none of the cases match the input value, the default branch will be taken
```js

const language = "DE";
switch(language)
{
	case "AT": //If any of the cases is true, the next code block will be executed.
	case "CH":
	case "DE":
	{
		return "Hallo Welt!";
	}
	case "EN":
	default: //None of the cases above matched the input string. We default to english here.
	{
		return "Hello World!";
	}
}
```


## Loops

Structures to repeat blocks of code.

### while

Repeats a block of code until the condition is true
```js
while(true) //Condition
{
	/*BLOCK*/
}
```

___

### for

Repeats a block of code until the condition is not met.

```js
for(let i = 0; i < 10; i++) //for(variable definition; condition; step;)
{

}
```

___

### foreach

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

## Locking

Objects can be locked to ensure only a single 'thread' is operating on it at a time.

Syntax:
```js

function DoWork(myObj)
{
	lock(myObj)
	{
		//Do some things that need locking
	}
}


```

## Using

Sometimes it is nessecary to manually manage the lifetime of an object.
To make this easier, it is possible to create a scoped lifetime with a `using` block.

Syntax:
```js

function DoWork(disposable)
{
	//The Variable Definition HAS to be declared 'const'.
	using(const myDisposable = disposable)
	{
		//Do things with myDisposable, it will be disposed once the using block ends.
	}
}

function DoWork1(disposable)
{
	using const myDisposable = disposable;
	//Do things with myDisposable, it will be disposed once parent block ends.
}

```

To create an Object that is able to be used as a Disposable, it needs to have a visible `Dispose` function.

```js

class MyDisposable
{
	let bool disposed;
	function MyDisposable()
	{
		disposed = false;
	}

	function Dispose()
	{
		if(disposed)
		{
			return;
		}
		disposed = true;

		//Release unmanaged resources here.
	}
}

function WorkWithMyDisposable()
{
	const myDisposable = new MyDisposable();
	using(const MyDisposable disposable = myDisposable)
	{
		//Do some things..
		//myDisposable.disposed is false here.
	}

	//myDisposable.disposed is true here.
}

function WorkWithMyDisposable1(doWork)
{
	const myDisposable = new MyDisposable();
	
	if(doWork)
	{
		using const MyDisposable disposable = myDisposable;
		//Do some things..
		//myDisposable.disposed is false here.
	}
	//myDisposable.disposed is true here.
}

```

## Modules

BadScript2 Supports Modules, which are a way to organize and reuse code.

### Importing Modules

To import a module, use the `import` keyword.

```js
import MyModule from './MyModule.bs';
```
By Default the module path is relative to the current file, but you can also use absolute paths.

#### Importing Installed Modules

If you are using the [Commandline](../basics/Commandline.md) with the `Package Handler` startup app, you can import modules by their name only using the `<>` syntax

```js
import MyModule from '<MyModule>';
```
> This tries to import the module from the installed packages located at the directory returned by `bs settings Subsystems.Run.LibraryDirectory`

> Refer to the [Start Building Section](../basics/StartBuilding.md) for more information on how to install your own modules.

#### Building Modules

To expose a module to other files you can use the `export` keyword with *named* expressions (e.g. Functions, Variables, Classes, Interfaces).

```js
//Expors a function
export function MyFunction(MyClass cls)
{
  return cls.value;
}

//Exports a class
export class MyClass
{
    const value;
    function MyClass()
    {
      value = 42;
    }
}
```

Other files can then import the module and use the function.

```js
import MyModule from './MyModule.bs';
const cls = new MyModule.MyClass();
Console.WriteLine(MyModule.MyFunction(cls));
```

##### Default/Unnamed Exports

Sometimes you want to export a single object from a module.

If the value is a *named* expression, you need to use the `default` keyword to specify the default export.

```js
export default function MyFunction()
{
  return 42;
}
```

Other files can then import the module and use the function without specifying a name.

```js
import MyFunction from './MyModule.bs';
Console.WriteLine(MyFunction());
```

For unnamed expressions, you can use the `export` keyword without a name.

```js
export {
    MyFunction: () => 42,
    Value: 42
};
```

> If the unnamed expression gets assigned to a variable, you need to use the `default` keyword to specify the default export.

```js
const myTable = {
    MyFunction: () => 42,
    Value: 42
};
export default myTable;
```

___

### Import Resolution

The Runtime supports registering custom import handlers.

The `projects/PackageHandler` Project uses this feature to resolve module imports from installed packages.

A very simple import handler could look like this:

```js

const modules = {
    MyModule: {
        MyFunction: () => 42
    }
};

const prefix = "mycustomhandler://";


//All Import Handlers MUST implement the IImportHandler Interface
class MyCustomImportHandler : IImportHandler
{
    //Returns the path to the module without the prefix
    function string GetPath(string path!)
    {
        return path.Substring(prefix.Length, path.Length - prefix.Length);
    }

    //Part of the IImportHandler Interface, needs to return a unique hash for the handler and the given path
    function string GetHash(string path!)
    {
        return $"mycustomhandler://{path}";
    }

    //Part of the IImportHandler Interface, needs to return true if the handler can handle the given path
    function bool Has(string path!)
    {
        if(!path.StartsWith("mycustomhandler://"))
        {
            return false;
        }
        const p = GetPath(path);

        return p in modules; //Return true if the module exists
    }

    //Part of the IImportHandler Interface, needs to return the module for the given path, only gets called if Has returns true
    function any Get(string path!)
    {
        return modules[GetPath(path)];
    }
}

//Register the Import Handler
Runtime.RegisterImportHandler(new MyCustomImportHandler());
```

This handler would allow you to import modules using the `mycustomhandler://` prefix.

```js
import MyModule from 'mycustomhandler://MyModule';
Console.WriteLine(MyModule.MyFunction());
```

## Functions

Functions are named(or unnamed) procedures that can be executed multiple times.

### The basics

There are named functions
```js
function MyFunctionName()
{
	//Code
}
```

And unnamed functions
```js
function()
{
	//Code
}
```
> Unnamed functions can not be referenced after they are created. Its advised to assign them to a variable.

```js
let myFunc = function()
{
	//Code
}
```

#### Single line blocks

Sometimes, functions only consist of one expression. For this usecase, single line blocks exist.

```js

function Double(n) => n * 2; //Single Line Block Named Function

let add = function(a, b) => a + b; //Single Line Block Unnamed Function

```

#### Lambda Functions

For convenience the Single Line Functions can be expressed as lambda expressions.
Those functions can not define a return type.

```js

const Add = (a, b) => a + b; //Specifying Parameters with a parameter list.

const Double = x => x * 2; //Single Parameters can be specified without the brackets

const Double4 = () => Double(4); //If the function has no parameters the empty brackets must be specified.

```

### Type Safety

Functions in BadScript2 support parameter type safety.
It allows the runtime to check the types of the parameters prior to running the function code.

```js

function F(num i)
{
	//Code
}

F(10); //Works. 10 is a number
F(null); //Works. null does not have type constraints
F(""); //Crash. string != num

```

> To disallow `null`, add the `!` parameter modifier to the desired parameter.

```js

function F(num i!)
{
	//Code
}

F(10); //Works. 10 is a number
F(null); //Crash. ! modifier does not allow null values.
F(""); //Crash. string != num

```

### Parameters

Functions can have named input parameters.

#### Default

The Default input parameter consists of only a single word. The Name of the parameter.

```js
function F(x /*x is the input parameter*/)
{
	//Code
}

F(10); // X is 10 inside F
```

#### NullChecked

Prepend `!` after the parameter name to disallow the passing of `null`-values to that parameter.

```js
function F(x! /*x is the input parameter*/)
{
	//Code
}

F(10); // X is 10 inside F
F(null); //Crashes execution. x does not allow null-values
```


#### Optional

Prepend `?` after the parameter name to make the parameter optional.
Optional parameters can only have more optional parameters to their right.

```js
function F(x?)
{
	//Code
}

F(10); 	//X is 10 inside F
F(); 	// X is null inside F
		//Runtime automatically assigns null to the parameter if it is not specified
```

#### Rest Args

Prepend `*` after the parameter name to let the runtime put all additional function arguments into a single list.
There can only be one `*` parameter in a function. It has to be the last parameter in the parameter list.

```js
function F(x*)
{
	//Code
}

F(); //x is an empty array
F(10, 20, 30); //x is an array of the input parameters [10, 20, 30]
```

#### Mixing Parameter Modifiers

An optional nullchecked argument.
The parameter is only null if it has not been specified at all.

```js
function F(x!?) => x;
//OR
function F(x?!) => x;

F(10);
F();
F(null); //Crash
```

### Extension Properties

Functions have extension properties that can help with working with functions.

#### `Name`-Property

To get the (original) name of the function, use the `Name` property.

```js
function F() {}
let unnamed = function() {}
let name = F.Name; //"F"
name = unnamed.Name; //"<anonymous>"
```

#### `Parameters`-Property

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

#### `Invoke`-Property

A helper function that takes an argument array as input and invokes the function object with the parameters inside the array.

```js

function F(x!, y?, z*)
{
	//Code
}

F.Invoke([1, 2]); //x = 1, y = 2, z = []
F.Invoke([1, 2, 3, 4, 5]); //x = 1, y = 2, z = [3, 4, 5]

```

## Type System

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

### Property Visibility

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

### Inheritance Chains

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



### Checking inheritance

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

### Inheritance with `this`

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

### Member Visibility

Members/Properties of classes can be declared public, protected or private.

```js
class C
{
	const __privateVar; //Private. Only accessible in functions of C
	const _protectedVar; //Protected. Only accessible in functions of C and functions of inheriting classes
	const publicVar; //Accessible from everywhere

	function _ProtectedFunc()
	{

	}
}

class D : C
{
	function D()
	{
		const prot = base._protectedVar;
		base._ProtectedFunc();

		const priv = base.__privateVar; //Crashes
	}
}


const d = new D();

d._ProtectedFunc(); //Crashes
d._protectedVar; //Crashes
d.__privateVar; //Crashes

```

### Keywords

Inside Class Definitions there are 2 special keywords that can be used.

#### this

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

#### base

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

### Operator Overrides

#### Array Access(op_ArrayAccess)

If a function with the name `op_ArrayAccess` is defined inside an object, the runtime will call this function if it finds an array access operator.

```js

class C
{
	let _innerTable = {}
	function op_ArrayAccess(key)
	{
		return ref _innerTable[key]; //Return a reference to _innerTable[key]
	}
}

let c = new C();

c["A"] = true; //Assigns A = true
let v = c["A"]; //Assigns v = c["A"]


```

#### Invocation(op_Invoke)

If a function with the name `op_Invoke` is defined inside an object, the runtime will call this function if it finds an invocation operator.

```js

class C
{
	let _innerTable = {}
	function op_Invoke(key, value?)
	{
		if(value == null)
		{
			return _innerTable[key]; //Return _innerTable[key]	
		}
		_innerTable[key] = value;
		return null;
	}
}

let c = new C();

c("A", true); //Assigns A = true
let v = c("A"); //Assigns v = return value c("A")


```

#### Math Assignment(op_AddAssign/op_SubtractAssign/...)

If a function with the name `op_AddAssign`, `op_SubtractAssign`, `op_MultiplyAssign`, `op_DivideAssign` or `op_ModuloAssign`  is defined inside an object, the runtime will call this function if it finds a math assignment operator.

```js

class C
{
	let _innerList = [];
	function op_AddAssign(right)
	{
		_innerList.Add(right);
	}
	function op_SubtractAssign(right)
	{
		_innerList.Remove(right);
	}
}

let c = new C();
function F() {}

c += F; //Adds F to the _innerList

c -= F; //Removes F from the _innerList


```
## Error Handling

BadScript2 has error handling with exceptions.
You can throw exceptions
```js
throw "This is an error";
```

and you can catch exceptions
```js
function F() => throw "This is an error";
try
{
	F();
}
catch(e)
{
	Console.WriteLine(e); //Prints: "This is an error" + some runtime information such as file and line number.
}
```

### `finally`-Blocks

When using a `try`/`catch` block, it is possible to define a code block to run regardless of the result. This block is called the `finally`-Block

```js
function F() => throw "This is an error";
try
{
	F();
}
catch(e)
{
	Console.WriteLine(e); //Prints: "This is an error" + some runtime information such as file and line number.
}
finally
{
	Console.WriteLine("Ran the finally block!");
}
```

### The Error Object
Whenever an exception gets thrown. The runtime wraps the supplied object into an object
```js
{
	StackTrace: "", //StackTrace of where the error happened
	InnerError: {/*Error Object of inner exception*/},
	ErrorObject: "" //The actual object that is thrown by the user code
}
```

## Optimizing Tips


The BadScript2 Runtime contains a few optimizations that are opt-in but can substantially improve the execution time.

> To Measure Execution time use the `-b/--benchmark` flag when running a script with the console.

### Constant Function Optimization

If functions always return the same output for a given input, it is useful to use the `const` keyword on a function.
When the function is declared `const`, the runtime will cache the return value of a function. Effectivly eliminating recomputing the same value over and over again.

#### Example

Consider the Unoptimized Code
```js

function Sum(n)
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}

let sum10k = Sum(10000);
let sum10k2 = Sum(10000); //Function will be called twice
```

If the function `Sum` is declared `const`, the value will only be computed once. For every subsequent call to `Sum` with the same arguments, the cached value will be returned.

```js

const function Sum(n)
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}

let sum10k = Sum(10000);
let sum10k2 = Sum(10000); //Cached result of the first call will be used instead of invoking the function again
```

### Compiling Functions

If a function is computationally expensive, it can be worth to compile the function into a pseudo assembly.
```js

function SumSlow(n) //Default Function. No optimizations applied
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}

compiled function SumFast(n) //Function is compiled.
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}

```

> The majority of the performance boost is gained by replacing the for loop with a Relative Jump, therefore eliminating creating new iterators for the expressions.

#### Runtime Compilation

It is possible to compile functions after they have been defined by using the `Compiler.Compile(SumSlow, false)` function.

```js

function SumSlow(n) //Default Function. No optimizations applied
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}

const SumFast = Compiler.Compile(SumSlow, true);
```

___

#### Enable Fast Execution of compiled functions

Functions execution speed can be improved further by disabling the Operator Override feature of the runtime.
This is done by declaring the function as `compiled fast`.

```js

compiled fast function Sum(n) //Compiled Function with operator overrides disabled.
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}

```

##### Runtime Compilation

The effect of `compiled fast` can also be archived at runtime. By using the `Compiler.Compile(Sum, true)` function.


### Combining Optimizations

The `const` and `compile`/`compile fast` keywords can be used in combination.
This will yield the biggest performance improvement. But errors can be hard to debug.
```js

const compiled fast function Sum(n) //Cached, Compiled Function with operator overrides disabled.
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}
```

> The `const` keyword must always be infront of the `compile` keyword.

### Benchmark Results for `Sum`

Consider the Function

```js
function Sum(n)
{
    let r = 0;
    for(let i = 0; i < n; i++)
    {
        r += i;
    }
    return r;
}
```

> The command used to benchmark is `bs run -f .\sum.bs -b`

| Compile Level | Description | Time(n=1.000) | Time(n=10.000) | Time(n=100.000) | Time(n=1.000.000) |
| --- | --- | --- | --- | --- | --- |
| None | No Optimizations Applied | 103ms | 210ms | 928ms | 7724ms |
| Compiled | Function was Compiled | 121ms | 184ms | 582ms | 4214ms |
| CompiledFast | Function was Compiled and Operator Overloads are disabled | 118ms | 159ms | 428ms | 2699ms |

