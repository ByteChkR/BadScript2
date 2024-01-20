namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     The Packet Types
/// </summary>
public enum BadConsolePacketType : byte
{
    Write,
    Read,
    Color,
    Clear,
    Disconnect,
    HeartBeat,
    Hello,
}