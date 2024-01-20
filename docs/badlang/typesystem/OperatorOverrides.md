# Operator Overrides

## Array Access(op_ArrayAccess)

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

## Invocation(op_Invoke)

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

## Math Assignment(op_AddAssign/op_SubtractAssign/...)

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

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)

[C# Documentation](/index.html)