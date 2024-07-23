using System.CodeDom.Compiler;
namespace BadScript2.Runtime.Objects.Types.Interface;

/// <summary>
///     Contains information about the validation process for an object.
/// </summary>
public readonly struct BadInterfaceValidatorResult
{
	/// <summary>
	///     Is true if the object satisfies all constraints from all interfaces.
	/// </summary>
	public bool IsValid { get; }

	/// <summary>
	///     The errors that occured during the validation process.
	/// </summary>
	private readonly BadInterfaceValidatorError[] m_Errors;

	/// <summary>
	///     Creates a new result.
	/// </summary>
	/// <param name="errors"></param>
	public BadInterfaceValidatorResult(params BadInterfaceValidatorError[] errors)
    {
        IsValid = errors.Length == 0;
        m_Errors = errors;
    }


    /// <inheritdoc />
    public override string ToString()
    {
        IndentedTextWriter writer = new IndentedTextWriter(new StringWriter());
        writer.WriteLine($"Validator completed. Result: {(IsValid ? "Valid" : $"Invalid({m_Errors.Length} Errors)")}");
        writer.Indent++;

        foreach (BadInterfaceValidatorError error in m_Errors)
        {
            writer.WriteLine(error);
        }

        writer.Indent--;

        return writer.InnerWriter.ToString();
    }
}