using System.Text;

using BadScript2.Common;
using BadScript2.Runtime;

namespace BadScript2.Debugging
{
    public readonly struct BadDebuggerStep
    {
        public readonly object? StepSource;
        public readonly BadExecutionContext Context;
        public readonly BadSourcePosition Position;

        public BadDebuggerStep(BadExecutionContext context, BadSourcePosition position, object? stepSource)
        {
            Context = context;
            Position = position;
            StepSource = stepSource;
        }

        public string GetInfo()
        {
            return
                $"######################################\nDebug Step at {Position.GetPositionInfo()}\n\nStepSource: {StepSource}\n\nContext: {Context.Scope.Name}: {Context.Scope}\n\n######################################\n";
        }


        public string GetSourceView(out int topInSource, out int lineInSource,int lineDelta = 4) => GetSourceView(lineDelta, lineDelta, out topInSource, out lineInSource);
        public string GetSourceView(int top, int bottom, out int topInSource, out int lineInSource)
        {
            StringBuilder sb = new StringBuilder($"File: {Position.FileName}\n");
            string[] lines = GetLines(top, bottom, out topInSource, out lineInSource);
            for (int i = 0; i < lines.Length; i++)
            {
                int ln = topInSource + i;
                string line = lines[i].Trim();
                sb.AppendLine($"{(lineInSource==ln ? ">>" : ln)}\t| {line}");
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return GetInfo();
        }


        private string[] GetLines(int top, int bottom, out int topInSource, out int lineInSource)
        {
            lineInSource = 1;
            for (int i = 0; i < Position.Index; i++)
            {
                if (Position.Source[i] == '\n')
                {
                    lineInSource++;
                }
            }

            topInSource = Math.Max(1, lineInSource - top);
        
            string[] lines = Position.Source.Split('\n');


            List<string> lns = new List<string>();
            for (int i = topInSource-1; i < (topInSource-1) +Math.Min(top+bottom, lines.Length - (topInSource-1)); i++)
            {
                lns.Add(lines[i]);
            }

            return lns.ToArray();
        }
    }
}