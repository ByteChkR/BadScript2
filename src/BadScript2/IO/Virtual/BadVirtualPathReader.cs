using BadScript2.Utility;
namespace BadScript2.IO.Virtual;

/// <summary>
///     Implements Operations to read and manipulate File System Paths
/// </summary>
public static class BadVirtualPathReader
{
    public static string JoinPath(IEnumerable<string> parts)
    {
        return '/' + string.Join("/", parts);
    }

    public static bool IsDirectory(string path)
    {
        return path.EndsWith('/') || path.EndsWith('\\');
    }

    public static string[] SplitPath(string path)
    {
        string[] parts = path.Split('/', '\\');

        if (IsAbsolutePath(path))
        {
            parts = parts.Skip(1).ToArray();
        }

        return parts;
    }

    public static bool IsAbsolutePath(string path)
    {
        return path.StartsWith('/') || path.StartsWith('\\');
    }

    public static bool IsRootPath(string path)
    {
        return path is "/" or "\\";
    }

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
            result = new List<string>(currentDir.Split('/', '\\').Skip(1));
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