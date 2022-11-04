# Compression API Reference

The `Compression` api has the following Properties to convert strings into binary form and back.

> This Api exposes `Compression.Zip.FromDirectory` and `Compression.Zip.ToDirectory` which allows the user to write to the filesystem. Caution is advised when executing untrusted code.

```js
{
        Deflate: {
                Compress: function Compress(IBadString)
                Decompress: function Decompress(BadArray)
        }
        GZip: {
                Compress: function Compress(IBadString)
                Decompress: function Decompress(BadArray)
        }
        ZLib: {
                Compress: function Compress(IBadString)
                Decompress: function Decompress(BadArray)
        }
        Zip: {
                FromDirectory: function FromDirectory(String, String)
                ToDirectory: function ToDirectory(String, String)
        }
}
```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)