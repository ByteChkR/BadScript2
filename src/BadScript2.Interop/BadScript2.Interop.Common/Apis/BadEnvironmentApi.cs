using System.Collections;

using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Apis;

[BadInteropApi("Environment")]
internal partial class BadEnvironmentApi
{
    protected override void AdditionalData(BadTable target)
    {
        target.SetProperty("CommandLine", Environment.CommandLine);
        target.SetProperty("CurrentDirectory", Environment.CurrentDirectory);
        target.SetProperty("ExitCode", Environment.ExitCode);
        target.SetProperty("HasShutdownStarted", Environment.HasShutdownStarted);
        target.SetProperty("Is64BitOperatingSystem", Environment.Is64BitOperatingSystem);
        target.SetProperty("Is64BitProcess", Environment.Is64BitProcess);
        target.SetProperty("MachineName", Environment.MachineName);
        target.SetProperty("NewLine", Environment.NewLine);
        target.SetProperty("OSVersion", Environment.OSVersion.ToString());
        target.SetProperty("ProcessorCount", Environment.ProcessorCount);
        target.SetProperty("StackTrace", Environment.StackTrace);
        target.SetProperty("SystemDirectory", Environment.SystemDirectory);
        target.SetProperty("SystemPageSize", Environment.SystemPageSize);
        target.SetProperty("TickCount", Environment.TickCount);
        target.SetProperty("UserDomainName", Environment.UserDomainName);
        target.SetProperty("UserInteractive", Environment.UserInteractive);
        target.SetProperty("UserName", Environment.UserName);
        target.SetProperty("WorkingSet", Environment.WorkingSet);
        target.SetProperty("CurrentManagedThreadId", Environment.CurrentManagedThreadId);
    }

    [BadMethod(description: "Expands Environment Variables in a string")]
    [return: BadReturn("String with all Environment Variables Expanded")]
    private string ExpandEnvironmentVariables(
        [BadParameter(
            description: "A string containing the names of zero or more environment variables. Each environment variable is quoted with the percent sign character (%)."
        )]
        string s)
    {
        return Environment.ExpandEnvironmentVariables(s);
    }

    [BadMethod(description: "Gets the value of an Environment Variable")]
    [return: BadReturn("The value of the Environment Variable")]
    private string? GetEnvironmentVariable([BadParameter(description: "The name of the environment variable")] string s)
    {
        return Environment.GetEnvironmentVariable(s);
    }

    [BadMethod(description: "Sets the value of an Environment Variable")]
    private void SetEnvironmentVariable(
        [BadParameter(description: "The name of an environment variable")] string n,
        [BadParameter(description: "A value to assign to variable")] string v)
    {
        Environment.SetEnvironmentVariable(n, v);
    }

    [BadMethod(description: "Gets all Environment Variables")]
    [return: BadReturn("A Table containing all Environment Variables")]
    private BadTable GetEnvironmentVariables()
    {
        IDictionary d = Environment.GetEnvironmentVariables();
        BadTable t = new BadTable();

        foreach (DictionaryEntry de in d)
        {
            t.SetProperty(de.Key.ToString(), de.Value.ToString());
        }

        return t;
    }

    [BadMethod(description: "Gets the Commandline Arguments")]
    [return: BadReturn("An Array containing all Commandline Arguments")]
    private BadArray GetCommandLineArguments()
    {
        return new BadArray(Environment.GetCommandLineArgs().Select(x => (BadObject)x).ToList());
    }

    [BadMethod(description: "Gets the Logical Drives")]
    [return: BadReturn("An Array containing all Logical Drives")]
    private BadArray GetLogicalDrives()
    {
        return new BadArray(Environment.GetLogicalDrives().Select(x => (BadObject)x).ToList());
    }

    [BadMethod(description: "Exits the Application")]
    private void Exit([BadParameter(description: "The exit code to return to the operating system. Use 0 (zero) to indicate that the process completed successfully.")] decimal e)
    {
        Environment.Exit((int)e);
    }

    [BadMethod(description: "Terminates the process")]
    private void FailFast([BadParameter(description: "A message that explains why the process was terminated, or null if no explanation is provided.")] string m)
    {
        Environment.FailFast(m);
    }
}