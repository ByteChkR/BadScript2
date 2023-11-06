using System.Collections;
using System.Diagnostics;

using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Apis;

/// <summary>
///     Implements the "OS" API
/// </summary>
public class BadOperatingSystemApi : BadInteropApi
{
	/// <summary>
	///     Constructs a new OS API Instance
	/// </summary>
	public BadOperatingSystemApi() : base("OS") { }

	/// <summary>
	///     Creates the "Environment" Table
	/// </summary>
	/// <returns>Bad Table</returns>
	private BadTable CreateEnvironmentTable()
    {
        BadTable env = new BadTable();

        env.SetProperty("CommandLine", Environment.CommandLine);
        env.SetProperty("CurrentDirectory", Environment.CurrentDirectory);
        env.SetProperty("ExitCode", Environment.ExitCode);
        env.SetProperty("HasShutdownStarted", Environment.HasShutdownStarted);
        env.SetProperty("Is64BitOperatingSystem", Environment.Is64BitOperatingSystem);
        env.SetProperty("Is64BitProcess", Environment.Is64BitProcess);
        env.SetProperty("MachineName", Environment.MachineName);
        env.SetProperty("NewLine", Environment.NewLine);
        env.SetProperty("OSVersion", Environment.OSVersion.ToString());
        env.SetProperty("ProcessorCount", Environment.ProcessorCount);
        env.SetProperty("StackTrace", Environment.StackTrace);
        env.SetProperty("SystemDirectory", Environment.SystemDirectory);
        env.SetProperty("SystemPageSize", Environment.SystemPageSize);
        env.SetProperty("TickCount", Environment.TickCount);
        env.SetProperty("UserDomainName", Environment.UserDomainName);
        env.SetProperty("UserInteractive", Environment.UserInteractive);
        env.SetProperty("UserName", Environment.UserName);
        env.SetProperty("WorkingSet", Environment.WorkingSet);
        env.SetProperty("CurrentManagedThreadId", Environment.CurrentManagedThreadId);
        env.SetFunction<string>("ExpandEnvironmentVariables", (_, s) => Environment.ExpandEnvironmentVariables(s));
        env.SetFunction<string>("GetEnvironmentVariable", (_, s) => Environment.GetEnvironmentVariable(s));
        env.SetFunction<string, string>("SetEnvironmentVariable", Environment.SetEnvironmentVariable);
        env.SetFunction(
            "GetEnvironmentVariables",
            () =>
            {
                IDictionary d = Environment.GetEnvironmentVariables();
                BadTable t = new BadTable();

                foreach (DictionaryEntry de in d)
                {
                    t.SetProperty(de.Key.ToString(), de.Value.ToString());
                }

                return t;
            }
        );
        env.SetFunction(
            "GetCommandlineArguments",
            () => { return new BadArray(Environment.GetCommandLineArgs().Select(x => (BadObject)x).ToList()); }
        );

        env.SetFunction(
            "GetLogicalDrives",
            () => { return new BadArray(Environment.GetLogicalDrives().Select(x => (BadObject)x).ToList()); }
        );
        env.SetFunction<decimal>("Exit", e => Environment.Exit((int)e));
        env.SetFunction<string>("FailFast", Environment.FailFast);

        return env;
    }


	/// <summary>
	///     Wrapper that creates an awaitable enumeration for a process
	/// </summary>
	/// <param name="p">The Process to wait for</param>
	/// <param name="r">Runnable Getter</param>
	/// <returns>Awaitable Enumeration</returns>
	private IEnumerator<BadObject> WaitForProcessExit(Process p, Func<BadInteropRunnable> r)
    {
        while (!p.HasExited)
        {
            yield return BadObject.Null;
        }

        r().SetReturn(p.ExitCode);
    }


	/// <summary>
	///     Creates the "Process" Table for a given Process
	/// </summary>
	/// <param name="p">The Process</param>
	/// <returns>Process Table</returns>
	private BadTable CreateProcessTable(Process p)
    {
        BadInteropRunnable r = null!;
        r = new BadInteropRunnable(WaitForProcessExit(p, () => r!));

        BadTable t = new BadTable();
        t.SetProperty("Awaitable", new BadTask(r, "WaitForProcessExit"));
        t.SetProperty("HasExited", p.HasExited);
        t.SetProperty("Id", p.Id);
        t.SetProperty("MachineName", p.MachineName);
        t.SetProperty("Responding", p.Responding);
        t.SetProperty("SessionId", p.SessionId);
        t.SetProperty("HandleCount", p.HandleCount);
        t.SetProperty("ProcessName", p.ProcessName);
        t.SetProperty("WorkingSet64", p.WorkingSet64);
        t.SetProperty("MainWindowTitle", p.MainWindowTitle);
        t.SetProperty("PagedMemorySize64", p.PagedMemorySize64);
        t.SetProperty("PagedSystemMemorySize64", p.PagedSystemMemorySize64);
        t.SetProperty("PeakPagedMemorySize64", p.PeakPagedMemorySize64);
        t.SetProperty("PeakVirtualMemorySize64", p.PeakVirtualMemorySize64);
        t.SetProperty("PeakWorkingSet64", p.PeakWorkingSet64);
        t.SetProperty("PrivateMemorySize64", p.PrivateMemorySize64);
        t.SetProperty("VirtualMemorySize64", p.VirtualMemorySize64);
        t.SetProperty("NonpagedSystemMemorySize64", p.NonpagedSystemMemorySize64);
        t.SetFunction("Kill", p.Kill);
        t.SetFunction("Refresh", p.Refresh);

        return t;
    }

    protected override void LoadApi(BadTable target)
    {
        target.SetProperty("Environment", CreateEnvironmentTable());
        target.SetFunction<string, string, string, bool, bool>(
            "Run",
            (f, a, w, cnoWnd, useShell) =>
            {
                Process p = new Process();
                p.StartInfo.FileName = f;
                p.StartInfo.Arguments = a;
                p.StartInfo.WorkingDirectory = w;
                p.StartInfo.CreateNoWindow = cnoWnd;
                p.StartInfo.UseShellExecute = useShell;
                p.Start();

                return CreateProcessTable(p);
            }
        );
    }
}