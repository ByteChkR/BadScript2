namespace BadScript2.Common;

/// <summary>
///     Contains Static Data for the BadScript Language
/// </summary>
public static class BadStaticKeys
{
	public static readonly char[] Whitespace =
	{
		' ',
		'\t'
	};

	public static readonly char[] NewLine =
	{
		'\r',
		'\n'
	};

	public static readonly char Quote = '"';
	public static readonly string FormatStringKey = "$\"";
	public static readonly string MultiLineStringKey = "@\"";
	public static readonly string MultiLineFormatStringKey = "$@\"";
	public static readonly string SingleLineComment = "//";
	public static readonly string MultiLineCommentStart = "/*";
	public static readonly string MultiLineCommentEnd = "*/";
	public static readonly string VariableDefinitionKey = "let";
	public static readonly string ConstantDefinitionKey = "const";
	public static readonly string StaticDefinitionKey = "static";
	public static readonly string CompiledDefinitionKey = "compiled";
	public static readonly string CompiledFastDefinitionKey = "fast";
	public static readonly string LockKey = "lock";
	public static readonly string True = "true";
	public static readonly string False = "false";
	public static readonly string Null = "null";
	public static readonly string NewKey = "new";
	public static readonly string FunctionKey = "function";
	public static readonly string ClassKey = "class";
	public static readonly string InterfaceKey = "interface";
	public static readonly string While = "while";
	public static readonly string ReturnKey = "return";
	public static readonly string RefKey = "ref";
	public static readonly string BreakKey = "break";
	public static readonly string ContinueKey = "continue";
	public static readonly string ThrowKey = "throw";
	public static readonly string IfKey = "if";
	public static readonly string ElseKey = "else";
	public static readonly string ForKey = "for";
	public static readonly string ForEachKey = "foreach";
	public static readonly string TryKey = "try";
	public static readonly string CatchKey = "catch";
	public static readonly char DecimalSeparator = '.';
	public static readonly char NegativeSign = '-';
	public static readonly char EscapeCharacter = '\\';
	public static readonly char StatementEndKey = ';';
	public static readonly char BlockEndKey = '}';


	public static readonly string ArrayAccessOperatorName = "op_ArrayAccess";
	public static readonly string ArrayAccessReverseOperatorName = "op_ArrayAccessReverse";
	public static readonly string InvocationOperatorName = "op_Invoke";
	public static readonly string AddAssignOperatorName = "op_AddAssign";
	public static readonly string SubtractAssignOperatorName = "op_SubtractAssign";
	public static readonly string MultiplyAssignOperatorName = "op_MultiplyAssign";
	public static readonly string ExponentiationAssignOperatorName = "op_ExponentiationAssign";
	public static readonly string DivideAssignOperatorName = "op_DivideAssign";
	public static readonly string ModuloAssignOperatorName = "op_ModuloAssign";
	public static readonly string AddOperatorName = "op_Add";
	public static readonly string SubtractOperatorName = "op_Subtract";
	public static readonly string MultiplyOperatorName = "op_Multiply";
	public static readonly string DivideOperatorName = "op_Divide";
	public static readonly string ModuloOperatorName = "op_Modulo";
	public static readonly string ExponentiationOperatorName = "op_Exponentiation";

	public static readonly string EqualOperatorName = "op_Equal";
	public static readonly string NotEqualOperatorName = "op_NotEqual";
	public static readonly string GreaterOperatorName = "op_Greater";
	public static readonly string GreaterEqualOperatorName = "op_GreaterOrEqual";
	public static readonly string LessOperatorName = "op_Less";
	public static readonly string LessEqualOperatorName = "op_LessOrEqual";


	public static readonly string InOperatorName = "op_In";
	public static readonly string NotOperatorName = "op_Not";
	public static readonly string PostDecrementOperatorName = "op_PostDecrement";
	public static readonly string PostIncrementOperatorName = "op_PostIncrement";
	public static readonly string PreDecrementOperatorName = "op_PreDecrement";
	public static readonly string PreIncrementOperatorName = "op_PreIncrement";

	public static IEnumerable<string> ReservedKeywords { get; set; } = new[]
	{
		VariableDefinitionKey,
		ConstantDefinitionKey,
		StaticDefinitionKey,
		LockKey,
		True,
		False,
		Null,
		NewKey,
		FunctionKey,
		ClassKey,
		InterfaceKey,
		While,
		ReturnKey,
		BreakKey,
		ContinueKey,
		ThrowKey,
		IfKey,
		ElseKey,
		ForKey,
		ForEachKey,
		TryKey,
		CatchKey,
		RefKey
	};

	public static bool IsReservedKeyword(string keyword)
	{
		return ReservedKeywords.Contains(keyword);
	}
}
