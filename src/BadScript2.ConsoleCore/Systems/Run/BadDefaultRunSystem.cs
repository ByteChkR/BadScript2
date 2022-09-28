namespace BadScript2.ConsoleCore.Systems.Run
{
    public class BadDefaultRunSystem : BadRunSystem
    {
        public override object? Parse(string[] args)
        {
            BadRunSystemSettings settings = new BadRunSystemSettings();
            settings.Args = args.Skip(1);
            settings.Files = args.Take(1);

            return settings;
        }
    }
}