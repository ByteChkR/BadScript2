using BadScript2.Utility;

namespace BadScript2.IO.Virtual;

/// <summary>
///     Implements Operations to read and manipulate File System Paths
/// </summary>
public static class BadVirtualPathReader
{
    /// <summary>
    /// Joins the given parts of a path together
    /// </summary>
    /// <param name="parts">The parts of the path</param>
    /// <returns>The joined path</returns>
    public static string JoinPath(IEnumerable<string> parts)
    {
        return '/' + string.Join("/", parts);
    }

    /// <summary>
    /// Returns true if the given ends with a slash
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>>true if the given path ends with a slash</returns>
    public static bool IsDirectory(string path)
    {
        return path.EndsWith('/') || path.EndsWith('\\');
    }

    /// <summary>
    /// Splits the given path into its parts
    /// </summary>
    /// <param name="path">The path to split</param>
    /// <returns>The parts of the path</returns>
    public static string[] SplitPath(string path)
    {
        string[] parts = path.Split('/', '\\');

        if (IsAbsolutePath(path))
        {
            parts = parts.Skip(1)
                         .ToArray();
        }

        return parts;
    }

    /// <summary>
    /// Returns true if the given path is absolute
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>>True if the given path is absolute</returns>
    public static bool IsAbsolutePath(string path)
    {
        return path.StartsWith('/') || path.StartsWith('\\');
    }

    /// <summary>
    /// Returns true if the given path is the root pathj
    /// </summary>
    /// <param name="path">The path to check</param>
    /// <returns>>True if the given path is the root path</returns>
    public static bool IsRootPath(string path)
    {
        return path is "/" or "\\";
    }

    /// <summary>
    /// Resolves a relative path to an absolute path
    /// </summary>
    /// <param name="path">The Path to resolve
    /// <param name="currentDir">The current directory</param>
    /// <returns>The resolved path</returns>
    /// <exception cref="Exception">Gets thrown when the path is invalid</exception>
    public static string ResolvePath(string path, string currentDir)
    {
        string[] parts = SplitPath(path);

        if (parts.Length == 0)
        {
            return currentDir;
        }

        List<string> result;

        if (IsAbsolutePath(path) || currentDir == "/" || currentDir == "\\")
        {
            result = new List<string>();
        }
        else
        {
            result = new List<string>(currentDir.Split('/', '\\')
                                                .Skip(1)
                                     );
        }

        foreach (string t in parts)
        {
            switch (t)
            {
                case ".":
                    continue;
                case ".." when result.Count == 0:
                    throw new Exception("Can't go back from root");
                case "..":
                    result.RemoveAt(result.Count - 1);

                    break;
                default:
                    result.Add(t);

                    break;
            }
        }

        if (result.Count != 0 && string.IsNullOrEmpty(result[result.Count - 1]))
        {
            result.RemoveAt(result.Count - 1);
        }

        return "/" + string.Join("/", result);
    }
}