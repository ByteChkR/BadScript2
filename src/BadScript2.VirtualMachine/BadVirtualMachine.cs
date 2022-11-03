using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction;
using BadScript2.Interop.Common.Apis;
using BadScript2.Interop.Common.Task;
using BadScript2.Interop.IO;
using BadScript2.IO;
using BadScript2.IO.Virtual;
using BadScript2.Parser;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.VirtualMachine
{
    public class BadVirtualMachine : IDisposable

    {
        public readonly BadVirtualFileSystem FileSystem;
        public readonly BadVirtualMachineInfo Info;
        private bool m_IsExitRequested;
        private bool m_IsRebootRequested;

        public BadVirtualMachine(BadVirtualMachineInfo info)
        {
            Info = info;
            FileSystem = new BadVirtualFileSystem();
            foreach (BadFileSystemMount mount in info.Mounts)
            {
                mount.Mount(FileSystem);
            }
        }

        public void Dispose()
        {
            foreach (BadFileSystemMount mount in Info.Mounts)
            {
                mount.Unmount(FileSystem);
            }
        }

        private IEnumerable<BadObject> InteractiveShell(BadExecutionContext ctx, IBadConsole console)
        {
            console.WriteLine("BadVM BIOS Shell");
            while (true)
            {
                console.Write("bios>");
                Task<string> rlTask = console.ReadLineAsync();
                while (!rlTask.IsCompleted)
                {
                    yield return BadObject.Null;
                }

                string line = rlTask.Result;
                if (line == "exit")
                {
                    break;
                }

                foreach (BadObject o in ctx.Execute(BadSourceParser.Create("<bios-cli>", line).Parse()))
                {
                    yield return o;
                }
            }
        }

        public void SaveState()
        {
            foreach (BadFileSystemMount mount in Info.Mounts)
            {
                mount.Unmount(FileSystem);
            }
        }

        public void ForceExit()
        {
            m_IsExitRequested = true;
        }

        public void Reboot()
        {
            m_IsRebootRequested = true;
        }

        public IEnumerable<BadObject> Execute(IBadConsole console)
        {
            m_IsRebootRequested = true;
            while (m_IsRebootRequested)
            {
                m_IsRebootRequested = false;
                BadExecutionContextOptions options =
                    new BadExecutionContextOptions(BadExecutionContextOptions.Default.Apis.Where(x => x is not BadIOApi && x is not BadConsoleApi).ToArray());
                options.Apis.Add(new BadConsoleApi(console));
                options.Apis.Add(new BadIOApi(FileSystem));

                BadTaskRunner runner = new BadTaskRunner();
                options.Apis.Add(new BadTaskRunnerApi(runner));

                BadVirtualMachineApi vmApi = new BadVirtualMachineApi(this);
                options.Apis.Add(vmApi);

                BadExecutionContext ctx = options.Build();

                ctx.Scope.AddSingleton(runner);
                if (FileSystem.IsFile("startup.bs"))
                {
                    runner.AddTask(
                        new BadTask(
                            new BadInteropRunnable(
                                ctx.Execute(
                                        BadSourceParser.Create(
                                                "startup.bs",
                                                FileSystem.ReadAllText("startup.bs")
                                            )
                                            .Parse()
                                    )
                                    .GetEnumerator()
                            ),
                            "__VM_MAIN__"
                        ),
                        true
                    );
                }
                else
                {
                    runner.AddTask(
                        new BadTask(
                            new BadInteropRunnable(
                                InteractiveShell(ctx, console)
                                    .GetEnumerator()
                            ),
                            "__VM_MAIN__"
                        ),
                        true
                    );
                }

                while (!runner.IsIdle && !m_IsExitRequested && !m_IsRebootRequested)
                {
                    runner.RunStep();

                    yield return BadObject.Null;
                }
            }
        }
    }
}