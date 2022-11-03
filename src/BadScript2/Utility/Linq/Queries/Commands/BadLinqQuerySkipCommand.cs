using System.Collections;

namespace BadScript2.Utility.Linq.Queries.Commands
{
    internal class BadLinqQuerySkipCommand : BadLinqQueryCommand
    {
        public BadLinqQuerySkipCommand() : base(true, false, "Skip") { }

        public override IEnumerable Run(BadLinqQueryCommandData data)
        {
            return data.Data.Skip(int.Parse(data.Argument!));
        }
    }
}