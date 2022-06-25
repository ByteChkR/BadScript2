# Concurrent API Reference

The `Concurrent` api has the following Properties:

```
{
        Run: function Run(BadTask)
        GetCurrent: function GetCurrent()
        Create: function Create(BadFunction)
}
```

## The Task Object

The `BadTask` object has the following properties:
```js
{
        Name: "nameoftask"
        IsCompleted: False
        IsInactive: True
        IsPaused: False
        IsRunning: False
        ContinueWith: function ContinueWith(BadTask)
        Pause: function Pause()
        Resume: function Resume()
        Cancel: function Cancel()
}
```

> The Task Object is one of the additional Native Types and can be created in two ways.
> `let task = Concurrent.Create(function() {});`
> or by calling the constructor
> `let task = new Task("nameoftask", function() {});`

## Awaiting Tasks

Tasks can be awaited by using the `await` keyword.

```js

function F()
{
        //Do some stuff that takes a while

        return "My Return Value";
}
let myTask = new Task("myTask", F);

let value = await myTask; //Pause current task to wait for completion of "myTask".

Console.WriteLine(value); //Writes "My Return Value" to the console output

```

## Function Extensions

To reduce boilerplate code there exists an extension function called `AsTask` for all BadFunctions.

```js

function F(x, y)
{
        //Do some stuff that takes a while

        return x + y;
}

let task = F.AsTask("Hello ", "World"); //Runs F as Task with the arguments provided.

let value = await task;

```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)