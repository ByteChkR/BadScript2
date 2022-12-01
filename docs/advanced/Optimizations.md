# Optimizing Tips


The BadScript2 Runtime contains a few optimizations that are opt-in but can substantially improve the execution time.

> To Measure Execution time use the `-b/--benchmark` flag when running a script with the console.

## Constant Function Optimization

If functions always return the same output for a given input, it is useful to use the `const` keyword on a function.
When the function is declared `const`, the runtime will cache the return value of a function. Effectivly eliminating recomputing the same value over and over again.

### Example

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

## Compiling Functions

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

### Runtime Compilation

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

### Enable Fast Execution of compiled functions

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

#### Runtime Compilation

The effect of `compile fast` can also be archived at runtime. By using the `Compiler.Compile(Sum, true)` function.


## Combining Optimizations

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

## Benchmark Results for `Sum`

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

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)