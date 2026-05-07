using System.Collections;
using System.Diagnostics;

using BadScript2.Interop.Common;
using BadScript2.Interop.IO;
using BadScript2.Interop.Json;
using BadScript2.Interop.Linq;
using BadScript2.IO;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Settings;
using BadScript2.Utility.Linq;

namespace BadScript2.Tests;

[Category("Performance")]
[NonParallelizable]
public class BadPerformanceTests
{
    private static readonly BadSystemFileSystem s_FileSystem = new BadSystemFileSystem();

    private static string PerfTestDirectory => Path.Combine(TestContext.CurrentContext.TestDirectory, "tests", "perf");

    static BadPerformanceTests()
    {
        BadSettingsProvider.SetRootSettings(new BadSettings(string.Empty));
        BadSettingsProvider.RootSettings
                           .FindOrCreateProperty("Runtime.NativeOptimizations.UseConstantFunctionCaching")
                           .SetValue(true);
    }

    private static BadRuntime CreateRuntime()
    {
        return new BadRuntime()
               .UseCommonInterop()
               .UseLinqApi()
               .UseFileSystemApi(s_FileSystem)
               .UseJsonApi()
               .UseLocalModules(s_FileSystem);
    }

    private static BadRuntimeExecutionResult ExecutePerfScript(string fileName)
    {
        using BadRuntime runtime = CreateRuntime();
        string file = Path.Combine(PerfTestDirectory, fileName);

        return runtime.ExecuteFile(file, s_FileSystem);
    }

    private static void Measure(string name, int iterations, Action action, int warmupIterations = 1)
    {
        for (int i = 0; i < warmupIterations; i++)
        {
            action();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            action();
        }

        sw.Stop();

        long allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;
        double avgMs = sw.Elapsed.TotalMilliseconds / iterations;

        TestContext.Progress.WriteLine(
            $"{name}: iterations={iterations}, totalMs={sw.Elapsed.TotalMilliseconds:F2}, avgMs={avgMs:F2}, allocatedBytes={allocatedBytes}");
    }

    [Test]
    public void ParseStressScript_Baseline()
    {
        string file = Path.Combine(PerfTestDirectory, "ParseStress.bs");
        string source = s_FileSystem.ReadAllText(file);
        int expressionCount = 0;

        Measure("ParseStressScript", 5, () => { expressionCount = BadRuntime.Parse(source, file).Count(); });

        Assert.That(expressionCount, Is.GreaterThan(0));
    }

    [Test]
    public void ExecuteScopeLookupScript_Baseline()
    {
        BadObject result = BadObject.Null;

        Measure("ExecuteScopeLookupScript", 5, () => { result = ExecutePerfScript("ScopeLookup.bs").Exports ?? BadObject.Null; });

        Assert.That(result, Is.EqualTo((BadObject)21000));
    }

    [Test]
    public void ExecuteMemberAccessScript_Baseline()
    {
        BadObject result = BadObject.Null;

        Measure("ExecuteMemberAccessScript", 5, () => { result = ExecutePerfScript("MemberAccess.bs").Exports ?? BadObject.Null; });

        Assert.That(result, Is.EqualTo((BadObject)30000));
    }

    [Test]
    public void ExecuteInvocationScript_Baseline()
    {
        BadObject result = BadObject.Null;

        Measure("ExecuteInvocationScript", 5, () => { result = ExecutePerfScript("Invocation.bs").Exports ?? BadObject.Null; });

        Assert.That(result, Is.EqualTo((BadObject)5000));
    }

    [Test]
    public void ExecuteLoopsScript_Baseline()
    {
        BadObject result = BadObject.Null;

        Measure("ExecuteLoopsScript", 5, () => { result = ExecutePerfScript("Loops.bs").Exports ?? BadObject.Null; });

        Assert.That(result, Is.EqualTo((BadObject)500555));
    }

    [Test]
    public void ExecuteModulesScript_Baseline()
    {
        BadObject result = BadObject.Null;

        Measure("ExecuteModulesScript", 5, () => { result = ExecutePerfScript("Modules.bs").Exports ?? BadObject.Null; });

        Assert.That(result, Is.EqualTo((BadObject)500500));
    }

    [Test]
    public void ReflectionInteropMethodLoop_Baseline()
    {
        BadReflectedObject obj = new BadReflectedObject(new Version());
        BadFunction toString = (BadFunction)obj.GetProperty("ToString")
                                               .Dereference(null);
        using BadExecutionContext ctx = BadExecutionContext.Create(new BadInteropExtensionProvider());
        BadObject result = BadObject.Null;

        Measure("ReflectionInteropMethodLoop", 5, () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                result = toString.Invoke(Array.Empty<BadObject>(), ctx)
                                 .Last();
            }
        });

        Assert.That(result, Is.EqualTo((BadObject)"0.0"));
    }

    [Test]
    public void LinqWhereQuery_Baseline()
    {
        IEnumerable data = Enumerable.Range(0, 10_000);
        int result = 0;

        Measure("LinqWhereQuery", 5, () =>
        {
            result = data.Where("x=>x%2==0")
                         .Cast<object>()
                         .Count();
        });

        Assert.That(result, Is.EqualTo(5000));
    }
}
