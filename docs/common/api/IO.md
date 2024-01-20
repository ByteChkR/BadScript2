# IO API Reference

The `IO` api has the following Properties to interact with the Filesystem of the Runtime.

> This Api exposes Functions that can read and write to the filesystem. Caution is advised when executing untrusted code.
> The Filesystem Implementation can be manually set to a virtual, temporary, in-memory filesystem.

```
{
        Info: {
                Name: IO
                Version: 2023.10.8.0
                AssemblyName: BadScript2.Interop.IO
        }
        Path: {
                GetFileName: function GetFileName(String!)
                GetFileNameWithoutExtension: function GetFileNameWithoutExtension(String!)
                GetDirectoryName: function GetDirectoryName(String!)
                GetExtension: function GetExtension(String!)
                GetFullPath: function GetFullPath(String!)
                GetStartupPath: function GetStartupPath(String!)
                ChangeExtension: function ChangeExtension(String!, String!)
                Combine: function Combine(parts*)
        }
        Directory: {
                CreateDirectory: function CreateDirectory(String!)
                Exists: function Exists(String!)
                Delete: function Delete(String!, Boolean!)
                Move: function Move(src!, dst!, overwrite?!)
                Copy: function Copy(src!, dst!, overwrite?!)
                GetCurrentDirectory: function GetCurrentDirectory()
                SetCurrentDirectory: function SetCurrentDirectory(String!)
                GetStartupDirectory: function GetStartupDirectory()
                GetDirectories: function GetDirectories(String!, Boolean!)
                GetFiles: function GetFiles(String!, String!, Boolean!)
        }
        File: {
                WriteAllText: function WriteAllText(String!, String!)
                ReadAllText: function ReadAllText(String!)
                Exists: function Exists(String!)
                ReadAllLines: function ReadAllLines(String!)
                WriteAllLines: function WriteAllLines(String!, BadArray!)
                WriteAllBytes: function WriteAllBytes(String!, BadArray!)
                ReadAllBytes: function ReadAllBytes(String!)
                Delete: function Delete(String!)
                Move: function Move(src!, dst!, overwrite?!)
                Copy: function Copy(src!, dst!, overwrite?!)
        }
}
```

___

## Links

[Home](../../Readme.md)

[Getting Started](../../GettingStarted.md)