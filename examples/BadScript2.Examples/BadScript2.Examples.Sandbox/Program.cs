// See https://aka.ms/new-console-template for more information

using BadScript2.Interop.Common;
using BadScript2.Interop.IO;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using BadScript2.Parser;
using BadScript2.Runtime;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("Sandbox Example");

        // Executing Scripts with unknown content is a security risk if you expose APIs that allow scripts to interact with the host system.
        // A notably dangerous api is the FileSystem API which allows scripts to read and write files on the host system.

        // There a two ways to mitigate this risk:
        //   1. Dont add any dangerous APIs to the execution context.
        //   2. Use the Virtual FileSystem Abstraction. (this will be demonstrated here)

        // The Virtual FileSystem Abstraction allows you to provide a virtual file system to the script.
        // This virtual file system will be used by the script to read and write files.

        // This script will be used to print the contents of "MySecretFile.txt" to the console.
        string source = "Console.WriteLine(IO.File.ReadAllText(\"./MySecretFile.txt\"));";

        //Add Common extensions
        BadCommonInterop.AddExtensions();

        // Lets create a "dangerous" context
        BadExecutionContextOptions unsafeOptions = new BadExecutionContextOptions();

        //Add Common APIs
        unsafeOptions.AddApis(BadCommonInterop.Apis);

        unsafeOptions.AddApi(new BadIOApi());

        //This context exposes the FileSystem API which allows scripts to read and write files to the host machine.
        BadExecutionContext unsafeContext = unsafeOptions.Build();

        //Execute the Script. It will print out the contents of "MySecretFile.txt" to the console.
        unsafeContext.Run(BadSourceParser.Parse("<none>", source));

        // Now lets create a virtual file system
        IFileSystem fileSystem = new BadVirtualFileSystem();

        //Write the secret file to the virtual file system(just so the script does not crash when trying to access it)
        fileSystem.WriteAllText("./MySecretFile.txt", "YIKES YOU ARE IN A SANDBOX");

        BadExecutionContextOptions safeOptions = new BadExecutionContextOptions();

        //Add Common APIs
        safeOptions.AddApis(BadCommonInterop.Apis);

        safeOptions.AddApi(new BadIOApi(fileSystem));

        //This Context Exposes the FileSystem API but uses a virtual file system that is held in memory.
        //The Script that is executed can only read and write files that are in the virtual file system.
        BadExecutionContext safeContext = safeOptions.Build();

        //Execute the Script. It will print out the contents of "MySecretFile.txt" to the console.
        safeContext.Run(BadSourceParser.Parse("<none>", source));

        // To make the virtual filesystem persistent you can export it to a zip file
        using (FileStream fs = File.Create("MyVirtualFileSystem.zip"))
        {
            fileSystem.ExportZip(fs);
        }

        // You can also import a zip file to a virtual filesystem(this usually happens before a script is executed)
        using (FileStream fs = File.OpenRead("MyVirtualFileSystem.zip"))
        {
            fileSystem.ImportZip(fs);
        }
    }
}