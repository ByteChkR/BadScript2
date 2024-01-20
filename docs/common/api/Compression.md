# Compression API Reference

The `Compression` api has the following Properties to convert strings into binary form and back.

> This Api exposes `Compression.Zip.FromDirectory` and `Compression.Zip.ToDirectory` which allows the user to write to the filesystem. Caution is advised when executing untrusted code.

```
{
        Info: {
                Name: Compression
                Version: 2023.10.8.0
                AssemblyName: BadScript2.Interop.Compression
        }
        Deflate: {
                Compress: function Compress(IBadString!)
                Decompress: function Decompress(BadArray!)
        }
        GZip: {
                Compress: function Compress(IBadString!)
                Decompress: function Decompress(BadArray!)
        }
        ZLib: {
                Compress: function Compress(IBadString!)
                Decompress: function Decompress(BadArray!)
        }
        Zip: {
                FromDirectory: function FromDirectory(String!, String!)
                ToDirectory: function ToDirectory(String!, String!)
        }
        Base64: {
                Encode: function Encode(BadArray!)
                Decode: function Decode(IBadString!)
        }
}
```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)