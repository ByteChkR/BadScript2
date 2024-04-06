using BadScript2.ConsoleAbstraction;
using BadScript2.IO;
namespace BadScript2.Web.Frontend.Utils;

public class BadReplContext
{
    public event Action OnLoaded = delegate { };

    public IBadConsole Console { get; private set; }

    public BadRuntime Runtime { get; private set; }

    public IFileSystem FileSystem { get; private set; }

    private readonly Action<string> m_OnFileClick;
    public BadReplContext(Action<string> mOnFileClick)
    {
        m_OnFileClick = mOnFileClick;
    }

    public void Load(BadRuntime runtime, IBadConsole console, IFileSystem fileSystem)
    {
        Runtime = runtime;
        Console = console;
        FileSystem = fileSystem;
        OnLoaded.Invoke();
    }

    public void OpenFile(string file) => m_OnFileClick(file);
}