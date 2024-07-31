namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Gets used to disconnect the client and the server
/// </summary>
public class BadConsoleDisconnectPacket : BadConsolePacket
{
    /// <summary>
    ///     Static Instance of this Packet
    /// </summary>
    public static readonly BadConsoleDisconnectPacket Packet = new BadConsoleDisconnectPacket();

    /// <summary>
    ///     Private Constructor
    /// </summary>
    private BadConsoleDisconnectPacket() { }

    /// <inheritdoc />
    public override byte[] Serialize()
    {
        return new[] { (byte)BadConsolePacketType.Disconnect };
    }

    /// <summary>
    ///     Deserializes the Packet
    /// </summary>
    /// <param name="data">The Data Array</param>
    /// <returns>Bad Console Packet instance</returns>
    public new static BadConsoleDisconnectPacket Deserialize(byte[] data)
    {
        if (data.Length != 1)
        {
            throw new BadNetworkConsoleException("Invalid Disconnect Packet");
        }

        return Packet;
    }
}