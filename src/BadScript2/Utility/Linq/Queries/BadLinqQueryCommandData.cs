using System.Collections;

namespace BadScript2.Utility.Linq.Queries;

public class BadLinqQueryCommandData
{
    public BadLinqQueryCommandData(IEnumerable data, string? argument = null)
    {
        Data = data;
        Argument = argument;
    }

    public IEnumerable Data { get; set; }
    public string? Argument { get; set; }
}