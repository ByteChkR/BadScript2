using BadScript2.IO;

/// <summary>
/// Contains the 'run' console command implementation
/// </summary>
namespace BadScript2.ConsoleCore.Systems.Run;

/// <summary>
///     Default Run System
///     Is used to enable running scripts by simply typing "bs my/path/to/script.bs"
/// </summary>
public class BadDefaultRunSystem : BadRunSystem
{
    /// <summary>
    ///     Creates a new BadDefaultRunSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadDefaultRunSystem(Func<BadRuntime> runtime) : base(runtime) { }

    /// <inheritdoc />
    public override object Parse(string[] args)
    {
        BadRunSystemSettings settings = new BadRunSystemSettings { Args = args.Skip(1) };
        var fs = new BadSystemFileSystem();
        string file = args.First();
        settings.Files = new[] { file };
        settings.Parallelization = 1;

        if (fs.IsFile(file))
        {
            return settings;
        }

        string path = Path.Combine(BadConsoleDirectories.GetDataDirectory(fs), "subsystems", "run", "apps", file + ".bs");

        if (fs.IsFile(path))
        {
            settings.Files = new[] { path };
        }

        return settings;
    }
}