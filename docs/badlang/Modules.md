# Modules

BadScript2 Supports Modules, which are a way to organize and reuse code.

## Importing Modules

To import a module, use the `import` keyword.

```js
import MyModule from './MyModule.bs';
```
By Default the module path is relative to the current file, but you can also use absolute paths.

### Importing Installed Modules

If you are using the [Commandline](../basics/Commandline.md) with the `Package Handler` startup app, you can import modules by their name only using the `<>` syntax

```js
import MyModule from '<MyModule>';
```
> This tries to import the module from the installed packages located at the directory returned by `bs settings Subsystems.Run.LibraryDirectory`

> Refer to the [Start Building Section](../basics/StartBuilding.md) for more information on how to install your own modules.

### Building Modules

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

#### Default/Unnamed Exports

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

## Import Resolution

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

___

## Links

[Home](../Readme.md)

[Getting Started](../GettingStarted.md)