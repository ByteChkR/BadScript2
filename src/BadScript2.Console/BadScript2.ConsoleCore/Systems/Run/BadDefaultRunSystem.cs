using BadScript2.IO;

namespace BadScript2.ConsoleCore.Systems.Run;

/// <summary>
///     Default Run System
///     Is used to enable running scripts by simply typing "bs &lt;script&gt; &lt;args&gt;"
/// </summary>
public class BadDefaultRunSystem : BadRunSystem
{
    public BadDefaultRunSystem(BadRuntime runtime) : base(runtime) { }

    public override object Parse(string[] args)
    {
        BadRunSystemSettings settings = new BadRunSystemSettings
        {
            Args = args.Skip(1),
        };
        string file = args.First();
        settings.Files = new[]
        {
            file,
        };

        if (BadFileSystem.Instance.IsFile(file))
        {
            return settings;
        }

        string path = Path.Combine(BadConsoleDirectories.DataDirectory, "subsystems", "run", "apps", file + ".bs");

        if (BadFileSystem.Instance.IsFile(path))
        {
            settings.Files = new[]
            {
                path,
            };
        }

        return settings;
    }
}