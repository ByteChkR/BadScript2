using BadScript2.IO;

namespace BadScript2.ConsoleCore.Systems.Run
{
    public class BadDefaultRunSystem : BadRunSystem
    {
        public override object? Parse(string[] args)
        {
            BadRunSystemSettings settings = new BadRunSystemSettings();
            settings.Args = args.Skip(1);
            string file = args.First();
            settings.Files = new[] {file};

            if (!BadFileSystem.Instance.IsFile(file))
            {
                string path = Path.Combine(BadConsoleDirectories.DataDirectory, "subsystems", "run", "apps", file + ".bs");
                if(BadFileSystem.Instance.IsFile(path))
                {
                    settings.Files = new[] {path};
                }
            }

            return settings;
        }
    }
}