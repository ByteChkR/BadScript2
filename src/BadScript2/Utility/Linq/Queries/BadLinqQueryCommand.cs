using System.Collections;

namespace BadScript2.Utility.Linq.Queries;

public abstract class BadLinqQueryCommand
{
    protected BadLinqQueryCommand(bool hasArgument, bool isArgumentOptional, params string[] names)
    {
        Names = names;
        HasArgument = hasArgument;
        IsArgumentOptional = isArgumentOptional;
    }

    public IEnumerable<string> Names { get; }
    public bool HasArgument { get; }
    public bool IsArgumentOptional { get; }
    public abstract IEnumerable Run(BadLinqQueryCommandData data);
}