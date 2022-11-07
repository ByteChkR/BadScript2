using System;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote;

public class BadNetworkConsoleException : Exception
{
    public BadNetworkConsoleException(string message) : base(message) { }
}