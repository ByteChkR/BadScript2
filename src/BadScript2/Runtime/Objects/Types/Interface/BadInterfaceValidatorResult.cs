using System.CodeDom.Compiler;

namespace BadScript2.Runtime.Objects.Types;

public readonly struct BadInterfaceValidatorResult
{
	public bool IsValid { get; }

	private readonly BadInterfaceValidatorError[] m_Errors;

	public BadInterfaceValidatorResult(params BadInterfaceValidatorError[] errors)
	{
		IsValid = errors.Length == 0;
		m_Errors = errors;
	}

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
