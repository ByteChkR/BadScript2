namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
/// Describes the semantic role of a member inside a compiled class template.
/// </summary>
public enum BadCompiledClassMemberKind
{
    Unknown,
    Field,
    Property,
    Method,
    Constructor,
}

