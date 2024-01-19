# Using

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

```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)