# Locking

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

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)