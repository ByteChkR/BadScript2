using BadScript2.Interop.Common.Task;

namespace BadScript2.ConsoleCore.Systems.Shell;

public class BadShellSystem : BadConsoleSystem<BadShellSystemSettings>
{
	public override string Name => "shell";

	protected override int Run(BadShellSystemSettings settings)
	{
		BadShell shell = new BadShell();

		if (settings.Command != null)
		{
			BadTask? task = shell.Run(settings.Command);

			if (task != null)
			{
				shell.RunUntilComplete(task);
			}

			return -1;
		}

		return shell.Run();
	}
}
