using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.ConsoleCore.Systems.Shell;

public class BadShellApi : BadInteropApi
{
	private readonly BadShell m_Shell;

	public BadShellApi(BadShell shell) : base("Shell")
	{
		m_Shell = shell;
	}

	protected override void LoadApi(BadTable target)
	{
		target.SetFunction("Exit", () => m_Shell.Exit());
	}
}
