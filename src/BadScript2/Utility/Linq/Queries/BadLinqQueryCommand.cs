using System.Collections;
namespace BadScript2.Utility.Linq.Queries;

/// <summary>
///     Implements an Abstract Command for the BadLinqQuery.
/// </summary>
public abstract class BadLinqQueryCommand
{
	/// <summary>
	///     Creates a new Command with the specified names.
	/// </summary>
	/// <param name="hasArgument">Does the Command have an Argument?</param>
	/// <param name="isArgumentOptional">Is the Argument Optional?</param>
	/// <param name="names">The Command Names</param>
	protected BadLinqQueryCommand(bool hasArgument, bool isArgumentOptional, params string[] names)
    {
        Names = names;
        HasArgument = hasArgument;
        IsArgumentOptional = isArgumentOptional;
    }

	/// <summary>
	///     The Command Names.
	/// </summary>
	public IEnumerable<string> Names { get; }

	/// <summary>
	///     Does the Command have an Argument?
	/// </summary>
	public bool HasArgument { get; }

	/// <summary>
	///     Is the Argument Optional?
	/// </summary>
	public bool IsArgumentOptional { get; }

	/// <summary>
	///     Runs the Command.
	/// </summary>
	/// <param name="data">The Command Data</param>
	/// <returns>Manipulated Data</returns>
	public abstract IEnumerable Run(BadLinqQueryCommandData data);
}