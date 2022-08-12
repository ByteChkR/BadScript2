namespace BadScript2.IO;

public interface IFileSystem
{
    string GetStartupDirectory();
    bool Exists(string path);
    bool IsFile(string path);
    bool IsDirectory(string path);
    IEnumerable<string> GetFiles(string path, string extension, bool recursive);
    IEnumerable<string> GetDirectories(string path, bool recursive);
    void CreateDirectory(string path, bool recursive = false);
    void DeleteDirectory(string path, bool recursive);
    void DeleteFile(string path);

    string GetFullPath(string path);

    Stream OpenRead(string path);
    Stream OpenWrite(string path, BadWriteMode mode);
    string GetCurrentDirectory();
    void SetCurrentDirectory(string path);

    void Copy(string src, string dst, bool overwrite = true);
    void Move(string src, string dst, bool overwrite = true);
}