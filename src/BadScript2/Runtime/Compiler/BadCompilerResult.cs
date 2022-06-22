using BadScript2.Common.Logging;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler
{
    public class BadCompilerResult
    {
        private readonly List<BadInstruction> m_Instructions = new List<BadInstruction>();
        private readonly Dictionary<string, int> m_Labels = new Dictionary<string, int>();
        private readonly BadScope m_Scope;

        private readonly Queue<(string, Action)> m_WorkItems = new Queue<(string, Action)>();

        private int m_UniqueCounter;

        public BadCompilerResult(BadScope scope)
        {
            m_Scope = scope;
        }

        public BadScope GetScope()
        {
            return m_Scope;
        }

        public int GetInstructionLength()
        {
            return m_Instructions.Count;
        }

        public BadInstruction[] GetInstructions()
        {
            return m_Instructions.ToArray();
        }

        public int Emit(BadInstruction instr)
        {
            m_Instructions.Add(instr);

            return m_Instructions.Count - 1;
        }

        public void AddWorkItem(string name, Action a)
        {
            m_WorkItems.Enqueue((name, a));
        }

        public void ProcessWorkItems()
        {
            while (m_WorkItems.Count > 0)
            {
                (string name, Action a) = m_WorkItems.Dequeue();

                BadLogger.Log($"Running work item: {name}", "Runtime");
                a();
            }
        }

        public string UniqueLabel(string name)
        {
            return $"{name}_{m_UniqueCounter++}";
        }

        public int DefineLabel(string label)
        {
            if (m_Labels.ContainsKey(label))
            {
                throw new BadRuntimeException("Label already defined");
            }

            m_Labels.Add(label, m_Instructions.Count);

            return m_Instructions.Count;
        }

        public int GetLabel(string label)
        {
            if (!m_Labels.ContainsKey(label))
            {
                throw new BadRuntimeException("Label not defined");
            }

            return m_Labels[label];
        }

        public void SetArgument(int instruction, int argIndex, BadObject obj)
        {
            m_Instructions[instruction].Arguments[argIndex] = obj;
        }
    }
}