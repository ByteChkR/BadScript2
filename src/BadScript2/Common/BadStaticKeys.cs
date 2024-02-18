
/// <summary>
/// Contains Shared Data Structures and Functionality
/// </summary>
namespace BadScript2.Common;

/// <summary>
///     Contains Static Data for the BadScript Language
/// </summary>
public static class BadStaticKeys
{
    public const char QUOTE = '"';
    public const char SINGLE_QUOTE = '\'';
    public const string FORMAT_STRING_KEY = "$\"";
    public const string MULTI_LINE_STRING_KEY = "@\"";
    public const string MULTI_LINE_FORMAT_STRING_KEY = "$@\"";
    public const string SINGLE_LINE_COMMENT = "//";
    public const string MULTI_LINE_COMMENT_START = "/*";
    public const string MULTI_LINE_COMMENT_END = "*/";
    public const string VARIABLE_DEFINITION_KEY = "let";
    public const string CONSTANT_DEFINITION_KEY = "const";
    public const string STATIC_DEFINITION_KEY = "static";
    public const string COMPILED_DEFINITION_KEY = "compiled";
    public const string COMPILED_FAST_DEFINITION_KEY = "fast";
    public const string IMPORT_KEY = "import";
    public const string FROM_KEY = "from";
    public const string EXPORT_KEY = "export";
    public const string DELETE_KEY = "delete";
    public const string DEFAULT_KEY = "default";
    public const string LOCK_KEY = "lock";
    public const string TRUE = "true";
    public const string FALSE = "false";
    public const string NULL = "null";
    public const string NEW_KEY = "new";
    public const string FUNCTION_KEY = "function";
    public const string CLASS_KEY = "class";
    public const string INTERFACE_KEY = "interface";
    public const string WHILE = "while";
    public const string RETURN_KEY = "return";
    public const string REF_KEY = "ref";
    public const string BREAK_KEY = "break";
    public const string CONTINUE_KEY = "continue";
    public const string THROW_KEY = "throw";
    public const string IF_KEY = "if";
    public const string ELSE_KEY = "else";
    public const string FOR_KEY = "for";
    public const string FOR_EACH_KEY = "foreach";
    public const string TRY_KEY = "try";
    public const string CATCH_KEY = "catch";
    public const string FINALLY_KEY = "finally";
    public const string USING_KEY = "using";
    public const char DECIMAL_SEPARATOR = '.';
    public const char NEGATIVE_SIGN = '-';
    public const char ESCAPE_CHARACTER = '\\';
    public const char STATEMENT_END_KEY = ';';
    public const char BLOCK_END_KEY = '}';


    public const string ARRAY_ACCESS_OPERATOR_NAME = "op_ArrayAccess";
    public const string ARRAY_ACCESS_REVERSE_OPERATOR_NAME = "op_ArrayAccessReverse";
    public const string INVOCATION_OPERATOR_NAME = "op_Invoke";
    public const string ADD_ASSIGN_OPERATOR_NAME = "op_AddAssign";
    public const string SUBTRACT_ASSIGN_OPERATOR_NAME = "op_SubtractAssign";
    public const string MULTIPLY_ASSIGN_OPERATOR_NAME = "op_MultiplyAssign";
    public const string EXPONENTIATION_ASSIGN_OPERATOR_NAME = "op_ExponentiationAssign";
    public const string DIVIDE_ASSIGN_OPERATOR_NAME = "op_DivideAssign";
    public const string MODULO_ASSIGN_OPERATOR_NAME = "op_ModuloAssign";
    public const string ADD_OPERATOR_NAME = "op_Add";
    public const string SUBTRACT_OPERATOR_NAME = "op_Subtract";
    public const string MULTIPLY_OPERATOR_NAME = "op_Multiply";
    public const string DIVIDE_OPERATOR_NAME = "op_Divide";
    public const string MODULO_OPERATOR_NAME = "op_Modulo";
    public const string EXPONENTIATION_OPERATOR_NAME = "op_Exponentiation";
    public const string NEGATION_OPERATOR_NAME = "op_Negate";

    public const string EQUAL_OPERATOR_NAME = "op_Equal";
    public const string NOT_EQUAL_OPERATOR_NAME = "op_NotEqual";
    public const string GREATER_OPERATOR_NAME = "op_Greater";
    public const string GREATER_EQUAL_OPERATOR_NAME = "op_GreaterOrEqual";
    public const string LESS_OPERATOR_NAME = "op_Less";
    public const string LESS_EQUAL_OPERATOR_NAME = "op_LessOrEqual";


    public const string IN_OPERATOR_NAME = "op_In";
    public const string NOT_OPERATOR_NAME = "op_Not";
    public const string POST_DECREMENT_OPERATOR_NAME = "op_PostDecrement";
    public const string POST_INCREMENT_OPERATOR_NAME = "op_PostIncrement";
    public const string PRE_DECREMENT_OPERATOR_NAME = "op_PreDecrement";
    public const string PRE_INCREMENT_OPERATOR_NAME = "op_PreIncrement";

    public static readonly char[] Whitespace =
    {
        ' ',
        '\t',
    };

    public static readonly char[] NewLine =
    {
        '\r',
        '\n',
    };

    public static IEnumerable<string> ReservedKeywords { get; set; } = new[]
    {
        VARIABLE_DEFINITION_KEY,
        CONSTANT_DEFINITION_KEY,
        STATIC_DEFINITION_KEY,
        LOCK_KEY,
        TRUE,
        FALSE,
        NULL,
        NEW_KEY,
        FUNCTION_KEY,
        CLASS_KEY,
        INTERFACE_KEY,
        WHILE,
        RETURN_KEY,
        BREAK_KEY,
        CONTINUE_KEY,
        THROW_KEY,
        IF_KEY,
        ELSE_KEY,
        FOR_KEY,
        FOR_EACH_KEY,
        TRY_KEY,
        CATCH_KEY,
        REF_KEY,
    };

    public static bool IsReservedKeyword(string keyword)
    {
        return ReservedKeywords.Contains(keyword);
    }
}