using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Interop.Common.Task
{
    public class BadTaskRunnerApi : BadInteropApi
    {
        private readonly BadTaskRunner m_Runner;

        public BadTaskRunnerApi(BadTaskRunner runner) : base("Concurrent")
        {
            m_Runner = runner;
        }

        public override void Load(BadTable target)
        {
            target.SetFunction<BadTask>("Run", AddTask);
            target.SetFunction("GetCurrent", GetCurrentTask);
            target.SetFunction<BadFunction>("Create", CreateTask);
        }

        private BadObject GetCurrentTask()
        {
            return m_Runner.Current ?? BadObject.Null;
        }

        private void AddTask(BadTask task)
        {
            m_Runner.AddTask(task, true);
        }

        private BadObject CreateTask(BadExecutionContext caller, BadFunction func)
        {
            return BadTask.Create(func, caller, null);
        }
    }
}