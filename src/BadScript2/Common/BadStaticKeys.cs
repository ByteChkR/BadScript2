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

	public static readonly string EqualOperatorName = "op_Equal";
	public static readonly string NotEqualOperatorName = "op_NotEqual";
	public static readonly string GreaterOperatorName = "op_Greater";
	public static readonly string GreaterEqualOperatorName = "op_GreaterOrEqual";
	public static readonly string LessOperatorName = "op_Less";
	public static readonly string LessEqualOperatorName = "op_LessOrEqual";
}
