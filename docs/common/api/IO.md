# IO API Reference

The `IO` api has the following Properties to interact with the Filesystem of the Runtime.

> This Api exposes Functions that can read and write to the filesystem. Caution is advised when executing untrusted code.
> The Filesystem Implementation can be manually set to a virtual, temporary, in-memory filesystem.

```js
{
        Path: {
                GetFileName: function GetFileName(String)
                GetFileNameWithoutExtension: function GetFileNameWithoutExtension(String)
                GetDirectoryName: function GetDirectoryName(String)
                GetExtension: function GetExtension(String)
                GetFullPath: function GetFullPath(String)
                GetStartupPath: function GetStartupPath(String)
                ChangeExtension: function ChangeExtension(String, String)
                Combine: function Combine(parts*)
        }
        Directory: {
                CreateDirectory: function CreateDirectory(String)
                Exists: function Exists(String)
                Delete: function Delete(String, Boolean)
                Move: function Move(String, String, Boolean)
                GetCurrentDirectory: function GetCurrentDirectory()
                SetCurrentDirectory: function SetCurrentDirectory(String)
                GetStartupDirectory: function GetStartupDirectory()
                GetDirectories: function GetDirectories(String, Boolean)
                GetFiles: function GetFiles(String, String, Boolean)
        }
        File: {
                WriteAllText: function WriteAllText(String, String)
                ReadAllText: function ReadAllText(String)
                Exists: function Exists(String)
                ReadAllLines: function ReadAllLines(String)
                WriteAllLines: function WriteAllLines(String, BadArray)
                WriteAllBytes: function WriteAllBytes(String, BadArray)
                ReadAllBytes: function ReadAllBytes(String)
                Delete: function Delete(String)
                Copy: function Copy(String, String)
        }
}
```

___

## Links

[Home](https://bytechkr.github.io/BadScript2/)

[Getting Started](https://bytechkr.github.io/BadScript2/GettingStarted.html)

[C# Documentation](https://bytechkr.github.io/BadScript2/reference/index.html)