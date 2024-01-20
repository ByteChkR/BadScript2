namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     The Packet Types
/// </summary>
public enum BadConsolePacketType : byte
{
    /// <summary>
    /// Write Packet Type
    /// </summary>
    Write,
    /// <summary>
    /// Read Packet Type
    /// </summary>
    Read,
    /// <summary>
    /// Color Packet Type
    /// </summary>
    Color,
    /// <summary>
    /// Clear Packet Type
    /// </summary>
    Clear,
    /// <summary>
    /// Disconnect Packet Type
    /// </summary>
    Disconnect,
    /// <summary>
    /// HeartBeat Packet Type
    /// </summary>
    HeartBeat,
    /// <summary>
    /// Hello Packet Type
    /// </summary>
    Hello,
}