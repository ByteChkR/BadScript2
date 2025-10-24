using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using Microsoft.Extensions.FileSystemGlobbing;

namespace BadScript2.Interop.IO;

[BadInteropApi("Path", true)]
internal partial class BadPathApi
{
    /// <summary>
    ///     The FileSystem Instance
    /// </summary>
    private readonly IFileSystem m_FileSystem;

    /// <summary>
    /// Creates a new API Instance
    /// </summary>
    /// <param name="fileSystem">The File System Instance to use</param>
    public BadPathApi(IFileSystem fileSystem) : this()
    {
        m_FileSystem = fileSystem;
    }

    /// <summary>
    /// Returns the file name and extension of the specified path string.
    /// </summary>
    /// <param name="path">The Path to get the file name from.</param>
    /// <returns>The characters after the last directory character in path.</returns>
    [BadMethod(description: "Returns the file name and extension of the specified path string.")]
    [return: BadReturn("The characters after the last directory character in path.")]
    private string GetFileName([BadParameter(description: "The Path to get the file name from.")] string path)
    {
        return Path.GetFileName(path);
    }

    /// <summary>
    /// Returns the file name of the specified path string without the extension.
    /// </summary>
    /// <param name="path">The Path to get the file name from.</param>
    /// <returns>The string returned by GetFileName(string), minus the last period (.) and all characters following it.</returns>
    [BadMethod(description: "Returns the file name of the specified path string without the extension")]
    [return:
        BadReturn("The string returned by GetFileName(string), minus the last period (.) and all characters following it"
                 )]
    private string GetFileNameWithoutExtension(
        [BadParameter(description: "The Path to get the file name from.")] string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Returns the directory information for the specified path string.
    /// </summary>
    /// <param name="path">The Path to get the directory information from.</param>
    /// <returns>Directory information for path, or null if path denotes a root directory or is null. Returns Empty if path does not contain directory information.</returns>
    [BadMethod(description: "Returns the directory information for the specified path string")]
    [return:
        BadReturn("Directory information for path, or null if path denotes a root directory or is null. Returns Empty if path does not contain directory information."
                 )]
    private string? GetDirectoryName([BadParameter(description: "The path of a file or directory.")] string path)
    {
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// Returns the extension of the specified path string.
    /// </summary>
    /// <param name="path">The Path to get the extension from.</param>
    /// <returns>The extension of the specified path (including the period).</returns>
    [BadMethod(description: "Returns the extension of the specified path string.")]
    [return: BadReturn("The extension of the specified path (including the period).")]
    private string GetExtension([BadParameter(description: "The path of a file or directory.")] string path)
    {
        return Path.GetExtension(path);
    }

    /// <summary>
    /// Returns the absolute path for the specified path string.
    /// </summary>
    /// <param name="path">The path of a file or directory.</param>
    /// <returns>The fully qualified location of path, such as "C:\MyFile.txt" or "/home/user/myFolder".</returns>
    [BadMethod(description: "Returns the absolute path for the specified path string.")]
    [return: BadReturn("The fully qualified location of path, such as \"C:\\MyFile.txt\" or \"/home/user/myFolder\".")]
    private string GetFullPath([BadParameter(description: "The path of a file or directory.")] string path)
    {
        return m_FileSystem.GetFullPath(path);
    }

    /// <summary>
    /// Returns the Startup Directory.
    /// </summary>
    /// <returns>The Startup Directory</returns>
    [BadMethod(description: "Returns the Startup Directory.")]
    [return: BadReturn("The Startup Directory")]
    private string GetStartupPath()
    {
        return m_FileSystem.GetStartupDirectory();
    }

    /// <summary>
    /// Changes the extension of a path string.
    /// </summary>
    /// <param name="path">The path information to modify.</param>
    /// <param name="extension">The new extension (with or without a leading period). Specify null to remove an existing extension from path.</param>
    /// <returns>The modified path information.</returns>
    [BadMethod(description: "Changes the extension of a path string.")]
    [return: BadReturn("The modified path information.")]
    private string ChangeExtension([BadParameter(description: "The path information to modify.")] string path,
                                   [BadParameter(description:
                                                    "The new extension (with or without a leading period). Specify null to remove an existing extension from path"
                                                )]
                                   string extension)
    {
        return Path.ChangeExtension(path, extension);
    }

    /// <summary>
    /// Combines two or more strings into a path.
    /// </summary>
    /// <param name="parts">The Path Parts</param>
    /// <returns>The combined path.</returns>
    [BadMethod(description: "Combines two or more strings into a path.")]
    [return: BadReturn("The combined path.")]
    private string Combine([BadParameter(description: "The Path Parts")] params string[] parts)
    {
        return Path.Combine(parts);
    }

    /// <summary>
    /// Expands Patterns using the GlobFile Syntax.
    /// </summary>
    /// <param name="ctx">The Current Execution Context</param>
    /// <param name="patterns">The Patterns</param>
    /// <param name="workingDirectory">The Working Directory</param>
    /// <returns>The expanded files.</returns>
    /// <exception cref="BadRuntimeException">Generated if no patterns are specified</exception>
    [BadMethod(description: "Expands Patterns using the GlobFile Syntax.")]
    [return: BadReturn("The expanded files.")]
    private BadArray Expand(BadExecutionContext ctx, [BadParameter(description: "The Patterns")] string[] patterns, string? workingDirectory = null)
    {
        if(patterns.Length == 0)
        {
            throw BadRuntimeException.Create(ctx.Scope, "Must specify at least one pattern");
        }
        
        workingDirectory ??= m_FileSystem.GetCurrentDirectory();

        Matcher m = new Matcher();
        foreach (var pattern in patterns)
        {
            if (pattern.StartsWith("!"))
            {
                m.AddExclude(pattern.Substring(1));
            }
            else
            {
                m.AddInclude(pattern);
            }
        }

        var result = m.Match(workingDirectory, m_FileSystem.GetFiles(workingDirectory, "*", true));
        
        return new BadArray(result.Files.Select(x => (BadObject)x.Path).ToList());
    }
}