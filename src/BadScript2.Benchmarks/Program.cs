using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BadScript2.Common;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.VirtualMachine;
using BadScript2.Runtime.VirtualMachine.Compiler;

namespace BadScript2.Benchmarks;

internal static class Program
{
    private static readonly JsonSerializerOptions s_JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static int Main(string[] args)
    {
        try
        {
            BenchmarkOptions options = BenchmarkOptions.Parse(args);
            string srcRoot = FindSourceRoot();
            string repoRoot = Directory.GetParent(srcRoot)?.FullName
                              ?? throw new InvalidOperationException("Failed to resolve repository root.");
            string projectsRoot = Path.Combine(repoRoot, "projects");
            string artifactsDir = Path.Combine(srcRoot, "artifacts", "benchmarks");

            Directory.CreateDirectory(artifactsDir);

            if (options.RunMacroSuite)
            {
                string bsPath = ResolveBsPath(srcRoot, options.BsPath);

                Console.WriteLine($"Using bs binary: {bsPath}");
                Console.WriteLine($"Projects root:   {projectsRoot}");
                Console.WriteLine($"Warmup runs:     {options.WarmupRuns}");
                Console.WriteLine($"Measured runs:   {options.MeasuredRuns}");
                Console.WriteLine();

                string workspacesDir = Path.Combine(artifactsDir, "workspaces");
                Directory.CreateDirectory(workspacesDir);
                List<ProjectBenchmarkCase> cases = CreateDefaultCases(projectsRoot, workspacesDir, options.Target);
                List<ProjectBenchmarkResult> results = new();

                foreach (ProjectBenchmarkCase benchmarkCase in cases)
                {
                    ProjectBenchmarkResult result = RunBenchmarkCase(bsPath, benchmarkCase, options);
                    results.Add(result);
                    PrintSummary(result);
                }

                BenchmarkReport report = new BenchmarkReport(DateTimeOffset.UtcNow,
                                                             Environment.Version.ToString(),
                                                             bsPath,
                                                             options,
                                                             results
                                                            );
                string reportPath = Path.Combine(artifactsDir,
                                                 $"macrobench-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.json"
                                                );
                File.WriteAllText(reportPath, JsonSerializer.Serialize(report, s_JsonOptions));

                Console.WriteLine();
                Console.WriteLine($"Macro benchmark report written to {reportPath}");
            }

            if (options.RunMicroSuite)
            {
                MicroBenchmarkReport microReport = RunMicroBenchmarks(options);
                string reportPath = Path.Combine(artifactsDir,
                                                 $"microbench-{DateTimeOffset.UtcNow:yyyyMMdd-HHmmss}.json"
                                                );
                File.WriteAllText(reportPath, JsonSerializer.Serialize(microReport, s_JsonOptions));

                Console.WriteLine();
                Console.WriteLine($"Micro benchmark report written to {reportPath}");
            }

            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return 1;
        }
    }

    private static string FindSourceRoot()
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);

        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "BadScript2.sln")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Could not locate BadScript2.sln from the current base directory.");
    }

    private static string ResolveBsPath(string srcRoot, string? configuredPath)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            string absolute = Path.GetFullPath(configuredPath);

            if (!File.Exists(absolute))
            {
                throw new FileNotFoundException($"Configured bs binary was not found: {absolute}");
            }

            return absolute;
        }

        string[] candidates =
        [
            Path.Combine(srcRoot, "BadScript2.Console", "BadScript2.Console", "bin", "Release", "net10.0", "bs"),
            Path.Combine(srcRoot, "BadScript2.Console", "BadScript2.Console", "bin", "Release", "net8.0", "bs"),
            Path.Combine(srcRoot, "BadScript2.Console", "BadScript2.Console", "bin", "Debug", "net10.0", "bs"),
            Path.Combine(srcRoot, "BadScript2.Console", "BadScript2.Console", "bin", "Debug", "net8.0", "bs")
        ];

        string? candidate = candidates.FirstOrDefault(File.Exists);

        if (candidate == null)
        {
            throw new FileNotFoundException("Could not locate a built bs binary. Build BadScript2.Console first or pass --bs-path.");
        }

        return candidate;
    }

    private static List<ProjectBenchmarkCase> CreateDefaultCases(string projectsRoot, string workspacesDir, string target)
    {
        return
        [
            CreateCase(projectsRoot, workspacesDir, "SourceReader", target),
            CreateCase(projectsRoot, workspacesDir, "BuildSystem", target),
            CreateCase(projectsRoot, workspacesDir, "CommandlineParser", target)
        ];
    }

    private static ProjectBenchmarkCase CreateCase(string projectsRoot, string workspacesDir, string projectName, string target)
    {
        string sourceProjectDirectory = Path.Combine(projectsRoot, projectName);
        string projectFilePath = Path.Combine(sourceProjectDirectory, "project.bsproj");
        ProjectFile projectFile = JsonSerializer.Deserialize<ProjectFile>(File.ReadAllText(projectFilePath), s_JsonOptions)
                                  ?? throw new InvalidOperationException($"Failed to deserialize {projectFilePath}");

        if (projectFile.Build?.OutputFile == null)
        {
            throw new InvalidOperationException($"Project {projectName} does not define Build.OutputFile.");
        }

        string workspaceDirectory = Path.Combine(workspacesDir, $"{projectName}-{target}");

        return new ProjectBenchmarkCase(projectName,
                                        sourceProjectDirectory,
                                        workspaceDirectory,
                                        target,
                                        projectFile.Build.OutputFile
                                       );
    }

    private static ProjectBenchmarkResult RunBenchmarkCase(string bsPath,
                                                           ProjectBenchmarkCase benchmarkCase,
                                                           BenchmarkOptions options)
    {
        List<BenchmarkRun> warmups = new();
        List<BenchmarkRun> measured = new();

        Console.WriteLine($"Running {benchmarkCase.ProjectName} ({benchmarkCase.Target})");

        for (int i = 0; i < options.WarmupRuns; i++)
        {
            warmups.Add(RunBuild(bsPath, benchmarkCase, true));
        }

        for (int i = 0; i < options.MeasuredRuns; i++)
        {
            measured.Add(RunBuild(bsPath, benchmarkCase, false));
        }

        return new ProjectBenchmarkResult(benchmarkCase.ProjectName,
                                          benchmarkCase.SourceProjectDirectory,
                                          benchmarkCase.WorkspaceDirectory,
                                          benchmarkCase.Target,
                                          benchmarkCase.GetOutputFile(),
                                          warmups,
                                          measured,
                                          CalculateSummary(measured)
                                         );
    }

    private static BenchmarkRun RunBuild(string bsPath, ProjectBenchmarkCase benchmarkCase, bool warmup)
    {
        PrepareWorkspace(benchmarkCase);
        string outputFile = benchmarkCase.GetOutputFile();

        ProcessStartInfo psi = new()
        {
            FileName = bsPath,
            Arguments = $"build {benchmarkCase.Target}",
            WorkingDirectory = benchmarkCase.WorkspaceDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        using Process process = new() { StartInfo = psi };
        Stopwatch stopwatch = Stopwatch.StartNew();

        process.Start();
        string stdout = process.StandardOutput.ReadToEnd();
        string stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        stopwatch.Stop();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Benchmark command failed for {benchmarkCase.ProjectName} with exit code {process.ExitCode}\nSTDOUT:\n{stdout}\nSTDERR:\n{stderr}");
        }

        if (!File.Exists(outputFile))
        {
            throw new FileNotFoundException($"Expected output file was not produced: {outputFile}");
        }

        string outputHash = ComputeSha256(outputFile);
        long outputLength = new FileInfo(outputFile).Length;

        return new BenchmarkRun(warmup,
                                stopwatch.Elapsed.TotalMilliseconds,
                                process.ExitCode,
                                outputHash,
                                outputLength,
                                Truncate(stdout),
                                string.IsNullOrWhiteSpace(stderr) ? null : Truncate(stderr)
                               );
    }

    private static void PrepareWorkspace(ProjectBenchmarkCase benchmarkCase)
    {
        if (Directory.Exists(benchmarkCase.WorkspaceDirectory))
        {
            Directory.Delete(benchmarkCase.WorkspaceDirectory, true);
        }

        CopyDirectory(benchmarkCase.SourceProjectDirectory, benchmarkCase.WorkspaceDirectory);
    }

    private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
    {
        Directory.CreateDirectory(destinationDirectory);

        foreach (string file in Directory.GetFiles(sourceDirectory))
        {
            string destination = Path.Combine(destinationDirectory, Path.GetFileName(file));
            File.Copy(file, destination, true);
        }

        foreach (string directory in Directory.GetDirectories(sourceDirectory))
        {
            string destination = Path.Combine(destinationDirectory, Path.GetFileName(directory));
            CopyDirectory(directory, destination);
        }
    }

    private static string ComputeSha256(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        byte[] hash = SHA256.HashData(stream);

        return Convert.ToHexString(hash);
    }

    private static string Truncate(string text, int maxLength = 1200)
    {
        if (text.Length <= maxLength)
        {
            return text;
        }

        return text[..maxLength];
    }

    private static MicroBenchmarkReport RunMicroBenchmarks(BenchmarkOptions options)
    {
        string source = BuildMicroSource(options.MicroVmIterations);
        string sourceHash = ComputeSha256FromText(source);

        Console.WriteLine("Running microbenchmarks (parse/compile/vm-execute)");
        Console.WriteLine($"Warmup runs:     {options.WarmupRuns}");
        Console.WriteLine($"Measured runs:   {options.MeasuredRuns}");
        Console.WriteLine($"VM iterations:   {options.MicroVmIterations}");
        Console.WriteLine();

         List<MicroBenchmarkStageResult> stages =
         [
             RunParseBenchmark("Baseline", source, options),
             RunCompileBenchmark("Baseline", source, options),
             RunVmExecuteBenchmark("VmExecute", "Baseline", source, options),
             RunVmOpcodeBenchmark("InvokeOpcode", options, BuildInvokeOpcodePayload),
             RunVmOpcodeBenchmark("InvokeCompiledOpcode", options, BuildInvokeCompiledOpcodePayload),
             RunVmOpcodeBenchmark("InvokeMemberOpcode", options, BuildInvokeMemberOpcodePayload),
             RunVmExecuteBenchmark("ClassInstantiation", "ClassInstantiation", BuildClassInstantiationMicroSource(options.MicroVmIterations), options),
             RunVmExecuteBenchmark("PropertyGetter", "PropertyGetter", BuildClassPropertyGetterMicroSource(options.MicroVmIterations), options),
             RunVmExecuteBenchmark("PropertySetter", "PropertySetter", BuildClassPropertySetterMicroSource(options.MicroVmIterations), options),
             RunVmExecuteBenchmark("InstanceMethodCall", "InstanceMethodCall", BuildClassInstanceMethodMicroSource(options.MicroVmIterations), options),
             RunVmExecuteBenchmark("StaticMethodCall", "StaticMethodCall", BuildClassStaticMethodMicroSource(options.MicroVmIterations), options),
             // AP4: Loop fast-path (IBadEnumerator direct access)
             RunVmExecuteBenchmark("ForeachLoop", "ForeachLoop", BuildForeachLoopMicroSource(), options, ctx => SeedForeachLoopArray(ctx, options.MicroVmIterations)),
             RunVmExecuteBenchmark("ForLoop", "ForLoop", BuildForLoopMicroSource(options.MicroVmIterations), options),
             RunVmExecuteBenchmark("WhileLoop", "WhileLoop", BuildWhileLoopMicroSource(options.MicroVmIterations), options),
             // AP5: LoadMember property reference cache
             RunVmExecuteBenchmark("LoadMemberField", "LoadMemberField", BuildLoadMemberFieldMicroSource(options.MicroVmIterations), options)
         ];

        foreach (MicroBenchmarkStageResult stage in stages)
        {
            PrintMicroSummary(stage);
        }

        List<MicroBenchmarkComparisonResult> comparisons = BuildOpcodeComparisons(stages);

        if (comparisons.Count > 0)
        {
            Console.WriteLine("Opcode delta report:");

            foreach (MicroBenchmarkComparisonResult comparison in comparisons)
            {
                PrintMicroComparison(comparison);
            }

            Console.WriteLine();
        }

        return new MicroBenchmarkReport(DateTimeOffset.UtcNow,
                                        Environment.Version.ToString(),
                                        options,
                                        source.Length,
                                        sourceHash,
                                        stages,
                                        comparisons
                                       );
    }

    private static MicroBenchmarkStageResult RunParseBenchmark(string sourceName, string source, BenchmarkOptions options)
    {
        Console.WriteLine($"Running parse benchmark for {sourceName}");
        string sourceHash = ComputeSha256FromText(source);

        static void Consume(IEnumerable<BadScript2.Parser.Expressions.BadExpression> expressions)
        {
            foreach (BadScript2.Parser.Expressions.BadExpression _ in expressions)
            {
            }
        }

        Console.WriteLine($"Running warmup for {sourceName}");
        List<double> warmup = new();
        for (int i = 0; i < options.WarmupRuns; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Consume(BadRuntime.Parse(source, "<micro-parse>"));
            sw.Stop();
            warmup.Add(sw.Elapsed.TotalMilliseconds);
        }

        Console.WriteLine($"Running measured for {sourceName}");
        List<double> measured = new();
        for (int i = 0; i < options.MeasuredRuns; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Consume(BadRuntime.Parse(source, "<micro-parse>"));
            sw.Stop();
            measured.Add(sw.Elapsed.TotalMilliseconds);
        }
        Console.WriteLine($"Completed parse benchmark for {sourceName}");

        return new MicroBenchmarkStageResult("Parse", sourceName, source.Length, sourceHash, warmup, measured, CalculateSummary(measured), null, null);
    }

    private static MicroBenchmarkStageResult RunCompileBenchmark(string sourceName, string source, BenchmarkOptions options)
    {
        Console.WriteLine($"Running compile benchmark for {sourceName}");
        BadScript2.Parser.Expressions.BadExpression[] parsed = BadRuntime.Parse(source, "<micro-compile>").ToArray();
        string sourceHash = ComputeSha256FromText(source);

        Console.WriteLine($"Running warmup for {sourceName}");
        List<double> warmup = new();
        int instructionCount = 0;

        for (int i = 0; i < options.WarmupRuns; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            BadInstruction[] instructions = BadCompiler.Compile(parsed).ToArray();
            sw.Stop();
            instructionCount = instructions.Length;
            warmup.Add(sw.Elapsed.TotalMilliseconds);
        }
        
        Console.WriteLine($"Running measured for {sourceName}");
        List<double> measured = new();
        for (int i = 0; i < options.MeasuredRuns; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            BadInstruction[] instructions = BadCompiler.Compile(parsed).ToArray();
            sw.Stop();
            instructionCount = instructions.Length;
            measured.Add(sw.Elapsed.TotalMilliseconds);
        }
        Console.WriteLine($"Completed compile benchmark for {sourceName}");

        return new MicroBenchmarkStageResult("Compile",
                                             sourceName,
                                             source.Length,
                                             sourceHash,
                                             warmup,
                                             measured,
                                             CalculateSummary(measured),
                                             instructionCount,
                                             null
                                            );
    }

    private static MicroBenchmarkStageResult RunVmExecuteBenchmark(string stageName,
                                                                   string sourceName,
                                                                   string source,
                                                                   BenchmarkOptions options,
                                                                   Action<BadExecutionContext>? setupContext = null)
    {
        Console.WriteLine($"Running {stageName} benchmark for {sourceName}");
        BadScript2.Parser.Expressions.BadExpression[] parsed = BadRuntime.Parse(source, "<micro-vm>").ToArray();
        BadInstruction[] instructions = BadCompiler.Compile(parsed).ToArray();
        BadSourcePosition position = BadSourcePosition.FromSource("micro-vm", 0, source.Length);

        using var runtime = new BadRuntime();
        using BadExecutionContext templateContext = runtime.CreateContext(Directory.GetCurrentDirectory());
        setupContext?.Invoke(templateContext);
        string sourceHash = ComputeSha256FromText(source);

        BadCompiledFunction compiledFunction = new BadCompiledFunction(instructions,
                                                                       true,
                                                                       templateContext.Scope,
                                                                       position,
                                                                       null,
                                                                       false,
                                                                       false,
                                                                       null,
                                                                       BadAnyPrototype.Instance,
                                                                       false
                                                                      );

        Console.WriteLine($"Running warmup for {sourceName}");
        List<double> warmup = new();
        for (int i = 0; i < options.WarmupRuns; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            ExecuteVm(compiledFunction, instructions, runtime, setupContext);
            sw.Stop();
            warmup.Add(sw.Elapsed.TotalMilliseconds);
        }
        
        Console.WriteLine($"Running measured for {sourceName}");
        List<double> measured = new();
        long evalFallbackCount = 0;
        for (int i = 0; i < options.MeasuredRuns; i++)
        {
            BadRuntimeVirtualMachine.ResetEvalInstructionCounter();
            Stopwatch sw = Stopwatch.StartNew();
            ExecuteVm(compiledFunction, instructions, runtime, setupContext);
            sw.Stop();
            measured.Add(sw.Elapsed.TotalMilliseconds);
            evalFallbackCount = Math.Max(evalFallbackCount, BadRuntimeVirtualMachine.EvalInstructionCount);
        }
        
        Console.WriteLine($"Completed {stageName} benchmark for {sourceName}");

        return new MicroBenchmarkStageResult(stageName,
                                             sourceName,
                                             source.Length,
                                             sourceHash,
                                             warmup,
                                             measured,
                                             CalculateSummary(measured),
                                             instructions.Length,
                                             evalFallbackCount
                                            );
    }

    private static void ExecuteVm(BadCompiledFunction compiledFunction,
                                  BadInstruction[] instructions,
                                  BadRuntime runtime,
                                  Action<BadExecutionContext>? setupContext = null)
    {
        using BadExecutionContext context = runtime.CreateContext(Directory.GetCurrentDirectory());
        setupContext?.Invoke(context);
        BadRuntimeVirtualMachine vm = new BadRuntimeVirtualMachine(compiledFunction, instructions);

        foreach (BadObject _ in vm.Execute(context))
        {
        }
    }

    private static MicroBenchmarkStageResult RunVmOpcodeBenchmark(string stageName,
                                                                  BenchmarkOptions options,
                                                                  Func<BadScope, BadSourcePosition, int, BadInstruction[]> payloadBuilder)
    {
        Console.WriteLine($"Running {stageName} opcode benchmark");
        using var runtime = new BadRuntime();
        using BadExecutionContext templateContext = runtime.CreateContext(Directory.GetCurrentDirectory());
        BadSourcePosition position = BadSourcePosition.FromSource($"micro-{stageName}", 0, stageName.Length);
        BadInstruction[] instructions = payloadBuilder(templateContext.Scope, position, options.MicroVmIterations);
        BadCompiledFunction compiledFunction = new BadCompiledFunction(instructions,
                                                                       true,
                                                                       templateContext.Scope,
                                                                       position,
                                                                       BadWordToken.MakeWord(stageName),
                                                                       false,
                                                                       false,
                                                                       null,
                                                                       BadAnyPrototype.Instance,
                                                                       false
                                                                      );

        Console.WriteLine($"Running warmup for {stageName}");
        List<double> warmup = new();
        for (int i = 0; i < options.WarmupRuns; i++)
        {
            Stopwatch sw = Stopwatch.StartNew();
            ExecuteVm(compiledFunction, instructions, runtime);
            sw.Stop();
            warmup.Add(sw.Elapsed.TotalMilliseconds);
        }

        
        Console.WriteLine($"Running measured for {stageName}");
        List<double> measured = new();
        long evalFallbackCount = 0;
        for (int i = 0; i < options.MeasuredRuns; i++)
        {
            BadRuntimeVirtualMachine.ResetEvalInstructionCounter();
            Stopwatch sw = Stopwatch.StartNew();
            ExecuteVm(compiledFunction, instructions, runtime);
            sw.Stop();
            measured.Add(sw.Elapsed.TotalMilliseconds);
            evalFallbackCount = Math.Max(evalFallbackCount, BadRuntimeVirtualMachine.EvalInstructionCount);
        }

        string sourceLabel = $"{stageName}:{options.MicroVmIterations}";

        
        Console.WriteLine($"Completed {stageName} opcode benchmark");
        
        return new MicroBenchmarkStageResult(stageName,
                                             "OpcodePayload",
                                             sourceLabel.Length,
                                             ComputeSha256FromText(sourceLabel),
                                             warmup,
                                             measured,
                                             CalculateSummary(measured),
                                             instructions.Length,
                                             evalFallbackCount
                                            );
    }

    private static BadInstruction[] BuildInvokeOpcodePayload(BadScope parentScope,
                                                             BadSourcePosition position,
                                                             int iterations)
    {
        BadCompiledFunction callee = CreateTrivialCompiledFunction(parentScope, position, "InvokeCallee");
        List<BadInstruction> instructions = new(iterations * 3 + 1);

        for (int i = 0; i < iterations; i++)
        {
            instructions.Add(new BadInstruction(BadOpCode.Push, position, callee));
            instructions.Add(new BadInstruction(BadOpCode.Invoke, position, 0));
            instructions.Add(new BadInstruction(BadOpCode.Pop, position));
        }

        instructions.Add(new BadInstruction(BadOpCode.Push, position, BadObject.Null));

        return instructions.ToArray();
    }

    private static BadInstruction[] BuildInvokeCompiledOpcodePayload(BadScope parentScope,
                                                                     BadSourcePosition position,
                                                                     int iterations)
    {
        BadCompiledFunction callee = CreateTrivialCompiledFunction(parentScope, position, "InvokeCompiledCallee");
        List<BadInstruction> instructions = new(iterations * 3 + 1);

        for (int i = 0; i < iterations; i++)
        {
            instructions.Add(new BadInstruction(BadOpCode.Push, position, callee));
            instructions.Add(new BadInstruction(BadOpCode.InvokeCompiled, position, 0));
            instructions.Add(new BadInstruction(BadOpCode.Pop, position));
        }

        instructions.Add(new BadInstruction(BadOpCode.Push, position, BadObject.Null));

        return instructions.ToArray();
    }

    private static BadInstruction[] BuildInvokeMemberOpcodePayload(BadScope parentScope,
                                                                   BadSourcePosition position,
                                                                   int iterations)
    {
        BadCompiledFunction method = CreateTrivialCompiledFunction(parentScope, position, "Run");
        BadTable target = new BadTable(new Dictionary<string, BadObject>
        {
            { "Run", method },
        });
        List<BadInstruction> instructions = new(iterations * 3 + 1);

        for (int i = 0; i < iterations; i++)
        {
            instructions.Add(new BadInstruction(BadOpCode.Push, position, target));
            instructions.Add(new BadInstruction(BadOpCode.InvokeMember, position, 0, "Run", false));
            instructions.Add(new BadInstruction(BadOpCode.Pop, position));
        }

        instructions.Add(new BadInstruction(BadOpCode.Push, position, BadObject.Null));

        return instructions.ToArray();
    }

    private static BadCompiledFunction CreateTrivialCompiledFunction(BadScope parentScope,
                                                                     BadSourcePosition position,
                                                                     string name)
    {
        BadInstruction[] body =
        [
            new BadInstruction(BadOpCode.Push, position, new BadNumber(1)),
            new BadInstruction(BadOpCode.Return, position, false),
        ];

        return new BadCompiledFunction(body,
                                       true,
                                       parentScope,
                                       position,
                                       BadWordToken.MakeWord(name),
                                       false,
                                       false,
                                       null,
                                       BadAnyPrototype.Instance,
                                       false
                                      );
    }

    private static string BuildMicroSource(int vmIterations)
    {
        return $"let i = 0;\n" +
               "let acc = 0;\n" +
               $"while(i < {vmIterations}){{\n" +
               "    acc = acc + ((i * 3) % 17);\n" +
               "    i = i + 1;\n" +
               "}\n" +
               "acc;\n";
    }

    private static string BuildClassMicroSource(string body)
    {
        return "class MicroBenchClass\n" +
               "{\n" +
               "    let num _value = 1;\n" +
               "    let num Value { get => _value; set => _value = value; }\n" +
               "    function MicroBenchClass()\n" +
               "    {\n" +
               "        _value = 1;\n" +
               "    }\n" +
               "    function num GetValue() => _value;\n" +
               "    static function num GetStaticValue() => 1;\n" +
               "}\n" +
               body;
    }

    private static string BuildClassInstantiationMicroSource(int vmIterations)
    {
        return BuildClassMicroSource($"let i = 0;\n" +
                                     "let sink = null;\n" +
                                     $"while(i < {vmIterations}){{\n" +
                                     "    sink = new MicroBenchClass();\n" +
                                     "    i = i + 1;\n" +
                                     "}\n" +
                                     "sink;\n");
    }

    private static string BuildClassPropertyGetterMicroSource(int vmIterations)
    {
        return BuildClassMicroSource($"let i = 0;\n" +
                                     "let acc = 0;\n" +
                                     "let c = new MicroBenchClass();\n" +
                                     $"while(i < {vmIterations}){{\n" +
                                     "    acc = acc + c.Value;\n" +
                                     "    i = i + 1;\n" +
                                     "}\n" +
                                     "acc;\n");
    }

    private static string BuildClassPropertySetterMicroSource(int vmIterations)
    {
        return BuildClassMicroSource($"let i = 0;\n" +
                                     "let c = new MicroBenchClass();\n" +
                                     $"while(i < {vmIterations}){{\n" +
                                     "    c.Value = i;\n" +
                                     "    i = i + 1;\n" +
                                     "}\n" +
                                     "c.Value;\n");
    }

    private static string BuildClassInstanceMethodMicroSource(int vmIterations)
    {
        return BuildClassMicroSource($"let i = 0;\n" +
                                     "let acc = 0;\n" +
                                     "let c = new MicroBenchClass();\n" +
                                     $"while(i < {vmIterations}){{\n" +
                                     "    acc = acc + c.GetValue();\n" +
                                     "    i = i + 1;\n" +
                                     "}\n" +
                                     "acc;\n");
    }

    private static string BuildClassStaticMethodMicroSource(int vmIterations)
    {
        return BuildClassMicroSource($"let i = 0;\n" +
                                     "let acc = 0;\n" +
                                     $"while(i < {vmIterations}){{\n" +
                                     "    acc = acc + MicroBenchClass.GetStaticValue();\n" +
                                     "    i = i + 1;\n" +
                                     "}\n" +
                                     "acc;\n");
    }

    private static void SeedForeachLoopArray(BadExecutionContext context, int vmIterations)
    {
        List<BadObject> values = new(vmIterations);

        for (int i = 0; i < vmIterations; i++)
        {
            values.Add(new BadNumber(i));
        }

        context.Scope.DefineVariable("arr", new BadArray(values));
    }

     private static string BuildForeachLoopMicroSource()
     {
         return "let acc = 0;\n" +
                "foreach(x in arr) { acc = acc + x; }\n" +
                "acc;\n";
     }

     private static string BuildForLoopMicroSource(int vmIterations)
     {
         return "let acc = 0;\n" +
                $"for(let i = 0; i < {vmIterations}; i = i + 1) {{\n" +
                "    acc = acc + i;\n" +
                "}\n" +
                "acc;\n";
     }

     private static string BuildWhileLoopMicroSource(int vmIterations)
     {
         return "let acc = 0;\n" +
                "let i = 0;\n" +
                $"while(i < {vmIterations}) {{\n" +
                "    acc = acc + i;\n" +
                "    i = i + 1;\n" +
                "}\n" +
                "acc;\n";
     }

    private static string BuildLoadMemberFieldMicroSource(int vmIterations)
    {
        // Reads a public field from a class instance in a tight while loop.
        // Exercises the LoadMember property-reference cache (AP5).
        return "class FieldBox {\n" +
               "    let num Num = 42;\n" +
               "}\n" +
               "let box = new FieldBox();\n" +
               "let acc = 0;\n" +
               $"let i = 0;\n" +
               $"while(i < {vmIterations}) {{\n" +
               "    acc = acc + box.Num;\n" +
               "    i = i + 1;\n" +
               "}\n" +
               "acc;\n";
    }

    private static string ComputeSha256FromText(string text)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        byte[] hash = SHA256.HashData(data);

        return Convert.ToHexString(hash);
    }

    private static BenchmarkSummary CalculateSummary(IReadOnlyList<double> runs)
    {
        double[] values = runs.OrderBy(x => x).ToArray();

        return new BenchmarkSummary(values.Min(),
                                    Median(values),
                                    values.Max(),
                                    values.Average()
                                   );
    }

    private static BenchmarkSummary CalculateSummary(List<BenchmarkRun> runs)
    {
        return CalculateSummary(runs.Select(x => x.DurationMs).ToArray());
    }

    private static double Median(IReadOnlyList<double> sortedValues)
    {
        if (sortedValues.Count == 0)
        {
            return 0;
        }

        int middle = sortedValues.Count / 2;

        return sortedValues.Count % 2 == 0
                   ? (sortedValues[middle - 1] + sortedValues[middle]) / 2.0
                   : sortedValues[middle];
    }

    private static void PrintSummary(ProjectBenchmarkResult result)
    {
        Console.WriteLine($"  median: {result.Summary.MedianMs:F2} ms");
        Console.WriteLine($"  min:    {result.Summary.MinMs:F2} ms");
        Console.WriteLine($"  max:    {result.Summary.MaxMs:F2} ms");
        Console.WriteLine($"  mean:   {result.Summary.MeanMs:F2} ms");
        Console.WriteLine($"  output: {result.OutputFile}");
        Console.WriteLine();
    }

    private static void PrintMicroSummary(MicroBenchmarkStageResult stage)
    {
        Console.WriteLine($"[{stage.Stage}] median: {stage.Summary.MedianMs:F3} ms");
        Console.WriteLine($"[{stage.Stage}] min:    {stage.Summary.MinMs:F3} ms");
        Console.WriteLine($"[{stage.Stage}] max:    {stage.Summary.MaxMs:F3} ms");
        Console.WriteLine($"[{stage.Stage}] mean:   {stage.Summary.MeanMs:F3} ms");

        if (stage.InstructionCount.HasValue)
        {
            Console.WriteLine($"[{stage.Stage}] instructions: {stage.InstructionCount.Value}");
        }

        if (stage.EvalFallbackCount.HasValue)
        {
            Console.WriteLine($"[{stage.Stage}] eval-fallbacks: {stage.EvalFallbackCount.Value}");
        }

        Console.WriteLine();
    }

    private static List<MicroBenchmarkComparisonResult> BuildOpcodeComparisons(IReadOnlyList<MicroBenchmarkStageResult> stages)
    {
        Dictionary<string, MicroBenchmarkStageResult> stageMap = stages.ToDictionary(x => x.Stage, StringComparer.OrdinalIgnoreCase);
        List<MicroBenchmarkComparisonResult> comparisons = new();

        if (stageMap.TryGetValue("InvokeOpcode", out MicroBenchmarkStageResult? invoke) &&
            stageMap.TryGetValue("InvokeCompiledOpcode", out MicroBenchmarkStageResult? invokeCompiled))
        {
            comparisons.Add(CreateComparison("InvokeCompiledOpcode", invoke, invokeCompiled));
        }

        if (stageMap.TryGetValue("InvokeOpcode", out invoke) &&
            stageMap.TryGetValue("InvokeMemberOpcode", out MicroBenchmarkStageResult? invokeMember))
        {
            comparisons.Add(CreateComparison("InvokeMemberOpcode", invoke, invokeMember));
        }

        // AP5: LoadMemberField vs InstanceMethodCall (both are per-iteration member access patterns)
        if (stageMap.TryGetValue("InstanceMethodCall", out MicroBenchmarkStageResult? instanceMethod) &&
            stageMap.TryGetValue("LoadMemberField", out MicroBenchmarkStageResult? loadMember))
        {
            comparisons.Add(CreateComparison("LoadMemberField", instanceMethod, loadMember));
        }

        return comparisons;
    }

    private static MicroBenchmarkComparisonResult CreateComparison(string targetStage,
                                                                    MicroBenchmarkStageResult baseline,
                                                                    MicroBenchmarkStageResult target)
    {
        double baselineMedian = baseline.Summary.MedianMs;
        double targetMedian = target.Summary.MedianMs;
        double deltaMs = targetMedian - baselineMedian;
        double deltaPercent = baselineMedian <= 0 ? 0 : (deltaMs / baselineMedian) * 100.0;

        return new MicroBenchmarkComparisonResult(targetStage,
                                                  baseline.Stage,
                                                  baselineMedian,
                                                  targetMedian,
                                                  deltaMs,
                                                  deltaPercent,
                                                  targetMedian <= 0 ? 0 : baselineMedian / targetMedian
                                                 );
    }

    private static void PrintMicroComparison(MicroBenchmarkComparisonResult comparison)
    {
        string sign = comparison.DeltaMs >= 0 ? "+" : "";
        Console.WriteLine($"[{comparison.TargetStage}] vs [{comparison.BaselineStage}]: {sign}{comparison.DeltaMs:F3} ms ({sign}{comparison.DeltaPercent:F1}%), x{comparison.SpeedupFactor:F2}");
    }
}

internal sealed record BenchmarkOptions(string? BsPath,
                                        int WarmupRuns,
                                        int MeasuredRuns,
                                        string Target,
                                        string Suite,
                                        int MicroVmIterations)
{
    public bool RunMacroSuite => Suite is "macro" or "all";
    public bool RunMicroSuite => Suite is "micro" or "all";

    public static BenchmarkOptions Parse(string[] args)
    {
        string? bsPath = null;
        int warmupRuns = 1;
        int measuredRuns = 7;
        string target = "default";
        string suite = "macro";
        int microVmIterations = 25000;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--bs-path":
                    bsPath = args[++i];
                    break;
                case "--warmup-runs":
                    warmupRuns = int.Parse(args[++i]);
                    break;
                case "--measured-runs":
                    measuredRuns = int.Parse(args[++i]);
                    break;
                case "--target":
                    target = args[++i];
                    break;
                case "--suite":
                    suite = args[++i].Trim().ToLowerInvariant();
                    break;
                case "--micro-vm-iterations":
                    microVmIterations = int.Parse(args[++i]);
                    break;
                case "--help":
                case "-h":
                    PrintHelp();
                    Environment.Exit(0);
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {args[i]}");
            }
        }

        if (suite is not ("macro" or "micro" or "all"))
        {
            throw new ArgumentException("--suite must be one of: macro, micro, all");
        }

        if (microVmIterations <= 0)
        {
            throw new ArgumentException("--micro-vm-iterations must be > 0");
        }

        return new BenchmarkOptions(bsPath, warmupRuns, measuredRuns, target, suite, microVmIterations);
    }

    private static void PrintHelp()
    {
        Console.WriteLine("BadScript2.Benchmarks");
        Console.WriteLine("  --suite <macro|micro|all> Suite to run (default: macro)");
        Console.WriteLine("  --bs-path <path>          Optional path to the bs executable");
        Console.WriteLine("  --warmup-runs <count>     Warmup runs per benchmark case (default: 1)");
        Console.WriteLine("  --measured-runs <count>   Measured runs per benchmark case (default: 7)");
        Console.WriteLine("  --target <name>           Build target to execute (default: default)");
        Console.WriteLine("  --micro-vm-iterations <n> Loop iterations for the VM execute microbench (default: 25000)");
    }
}

internal sealed record ProjectBenchmarkCase(string ProjectName,
                                            string SourceProjectDirectory,
                                            string WorkspaceDirectory,
                                            string Target,
                                            string OutputFileRelativePath
                                           )
{
    public string GetOutputFile() => Path.GetFullPath(Path.Combine(WorkspaceDirectory, OutputFileRelativePath));
}

internal sealed record BenchmarkRun(bool IsWarmup,
                                    double DurationMs,
                                    int ExitCode,
                                    string OutputSha256,
                                    long OutputBytes,
                                    string StandardOutput,
                                    string? StandardError
                                   );

internal sealed record BenchmarkSummary(double MinMs, double MedianMs, double MaxMs, double MeanMs);

internal sealed record ProjectBenchmarkResult(string ProjectName,
                                              string ProjectDirectory,
                                              string WorkspaceDirectory,
                                              string Target,
                                              string OutputFile,
                                              List<BenchmarkRun> WarmupRuns,
                                              List<BenchmarkRun> MeasuredRuns,
                                              BenchmarkSummary Summary
                                             );

internal sealed record BenchmarkReport(DateTimeOffset CreatedAtUtc,
                                       string DotNetVersion,
                                       string BsPath,
                                       BenchmarkOptions Options,
                                       List<ProjectBenchmarkResult> Results
                                      );

internal sealed record MicroBenchmarkReport(DateTimeOffset CreatedAtUtc,
                                            string DotNetVersion,
                                            BenchmarkOptions Options,
                                            int SourceLength,
                                            string SourceSha256,
                                            List<MicroBenchmarkStageResult> Stages,
                                            List<MicroBenchmarkComparisonResult> Comparisons
                                           );

internal sealed record MicroBenchmarkStageResult(string Stage,
                                                 string SourceName,
                                                 int SourceLength,
                                                 string SourceSha256,
                                                 List<double> WarmupRunsMs,
                                                 List<double> MeasuredRunsMs,
                                                 BenchmarkSummary Summary,
                                                 int? InstructionCount,
                                                 long? EvalFallbackCount
                                                );

internal sealed record MicroBenchmarkComparisonResult(string TargetStage,
                                                      string BaselineStage,
                                                      double BaselineMedianMs,
                                                      double TargetMedianMs,
                                                      double DeltaMs,
                                                      double DeltaPercent,
                                                      double SpeedupFactor
                                                     );

internal sealed record ProjectFile(ProjectBuildConfiguration? Build);

internal sealed record ProjectBuildConfiguration(string? OutputFile);
