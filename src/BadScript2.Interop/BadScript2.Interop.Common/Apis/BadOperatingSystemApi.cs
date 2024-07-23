using System.Diagnostics;

using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
namespace BadScript2.Interop.Common.Apis;

/// <summary>
///     Implements the "OS" API
/// </summary>
[BadInteropApi("OS")]
internal partial class BadOperatingSystemApi
{
    /// <summary>
    ///     Creates the "Environment" Table
    /// </summary>
    /// <returns>Bad Table</returns>
    private BadTable CreateEnvironmentTable()
    {
        BadTable env = new BadTable();

        new BadEnvironmentApi().LoadRawApi(env);

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

        // ReSharper disable once AccessToModifiedClosure
        r = new BadInteropRunnable(WaitForProcessExit(p, () => r));

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


    [BadMethod(description: "Runs a Process")]
    [return: BadReturn("Process Table")]
    private BadTable Run(
        [BadParameter(
            description:
            "The name of the application to start, or the name of a document of a file type that is associated with an application and that has a default open action available to it. The default is an empty string (\"\")."
        )]
        string fileName,
        [BadParameter(
            description:
            "A single string containing the arguments to pass to the target application specified in the FileName property. The default is an empty string (\"\"). On Windows Vista and earlier versions of the Windows operating system, the length of the arguments added to the length of the full path to the process must be less than 2080. On Windows 7 and later versions, the length must be less than 32699. Arguments are parsed and interpreted by the target application, so must align with the expectations of that application. For.NET applications as demonstrated in the Examples below, spaces are interpreted as a separator between multiple arguments. A single argument that includes spaces must be surrounded by quotation marks, but those quotation marks are not carried through to the target application. In include quotation marks in the final parsed argument, triple-escape each mark."
        )]
        string arguments,
        [BadParameter(
            description:
            "When UseShellExecute is true, the fully qualified name of the directory that contains the process to be started. When the UseShellExecute property is false, the working directory for the process to be started. The default is an empty string (\"\")"
        )]
        string workingDirectory,
        [BadParameter(description: "\ntrue if the process should be started without creating a new window to contain it; otherwise, false. The default is false.")]
        bool createNoWindow,
        [BadParameter(
            description: "true if the shell should be used when starting the process; false if the process should be created directly from the executable file. The default is true"
        )]
        bool useShellExecute)
    {
        Process p = new Process();
        p.StartInfo.FileName = fileName;
        p.StartInfo.Arguments = arguments;
        p.StartInfo.WorkingDirectory = workingDirectory;
        p.StartInfo.CreateNoWindow = createNoWindow;
        p.StartInfo.UseShellExecute = useShellExecute;
        p.Start();

        return CreateProcessTable(p);
    }

    protected override void AdditionalData(BadTable target)
    {
        target.SetProperty("Environment", CreateEnvironmentTable());
    }
}