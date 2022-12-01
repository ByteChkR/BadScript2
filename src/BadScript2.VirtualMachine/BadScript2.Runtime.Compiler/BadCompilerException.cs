using System;

namespace BadScript2.Compiler;

public class BadCompilerException : Exception
{
    public BadCompilerException(string message) : base(message) { }
}