namespace BadScript2.IO;

public class SubFileSystem : IFileSystem
{
    //This class wraps another filesystem and adds a subpath to all operations. Ensuring that the user only accesses files within that subpath.
    private readonly IFileSystem _inner;
    private readonly string _subPath;

    public SubFileSystem(IFileSystem inner, string subPath)
    {
        _inner = inner;
        _subPath = subPath;
        _inner.SetCurrentDirectory(_subPath);
    }

    public string GetStartupDirectory()
    {
        return _subPath;
    }

    public bool Exists(string path)
    {
        //Get Full Path, then combine with subpath
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        return _inner.Exists(fp);
    }

    public bool IsFile(string path)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        return _inner.IsFile(fp);
    }

    public bool IsDirectory(string path)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        return _inner.IsDirectory(fp);
    }

    public IEnumerable<string> GetFiles(string path, string extension, bool recursive)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        return _inner.GetFiles(fp, extension, recursive);
    }

    public IEnumerable<string> GetDirectories(string path, bool recursive)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        return _inner.GetDirectories(fp, recursive);
    }

    public void CreateDirectory(string path, bool recursive = false)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        _inner.CreateDirectory(fp, recursive);
    }

    public void DeleteDirectory(string path, bool recursive)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        _inner.DeleteDirectory(fp, recursive);
    }

    public void DeleteFile(string path)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        _inner.DeleteFile(fp);
    }

    public string GetFullPath(string path)
    {
        //Ensure that the path is within the subpath by removing any leading slashes and combining it with the subpath
        var fp = _inner.GetFullPath(path);
        if(!fp.StartsWith(_subPath))
        {
            throw new Exception("Accessing files outside of the subpath is not allowed");
        }
        //Remove the subpath from the full path
        var result = fp.Remove(0, _subPath.Length).Replace("\\", "/");
        return result;
    }

    public Stream OpenRead(string path)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        return _inner.OpenRead(fp);
    }

    public Stream OpenWrite(string path, BadWriteMode mode)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        return _inner.OpenWrite(fp, mode);
    }

    public string GetCurrentDirectory()
    {
        var dir= _inner.GetCurrentDirectory();
        if(!dir.StartsWith(_subPath))
        {
            throw new Exception("Accessing files outside of the subpath is not allowed");
        }
        return dir.Remove(0, _subPath.Length).Replace("\\", "/");
    }

    public void SetCurrentDirectory(string path)
    {
        var p = GetFullPath(path);
        var fp = Path.Combine(_subPath, p.TrimStart('/', '\\')).Replace('\\', '/');
        _inner.SetCurrentDirectory(fp);
    }

    public void Copy(string src, string dst, bool overwrite = true)
    {
        var pSrc = GetFullPath(src);
        var pDst = GetFullPath(dst);
        var fpSrc = Path.Combine(_subPath, pSrc.TrimStart('/', '\\')).Replace('\\', '/');
        var fpDst = Path.Combine(_subPath, pDst.TrimStart('/', '\\')).Replace('\\', '/');
        _inner.Copy(fpSrc, fpDst, overwrite);
    }

    public void Move(string src, string dst, bool overwrite = true)
    {
        var pSrc = GetFullPath(src);
        var pDst = GetFullPath(dst);
        var fpSrc = Path.Combine(_subPath, pSrc.TrimStart('/', '\\')).Replace('\\', '/');
        var fpDst = Path.Combine(_subPath, pDst.TrimStart('/', '\\')).Replace('\\', '/');
        _inner.Move(fpSrc, fpDst, overwrite);
    }
}