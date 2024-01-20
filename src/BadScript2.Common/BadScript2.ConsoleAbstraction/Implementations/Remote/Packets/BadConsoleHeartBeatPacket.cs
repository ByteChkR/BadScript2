namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Gets used to send a HeartBeat to the client
/// </summary>
public class BadConsoleHeartBeatPacket : BadConsolePacket
{
	/// <summary>
	///     Static Instance of this Packet
	/// </summary>
	public static readonly BadConsoleHeartBeatPacket Packet = new BadConsoleHeartBeatPacket();

	/// <summary>
	///     Private Constructor
	/// </summary>
	private BadConsoleHeartBeatPacket() { }

    public override byte[] Serialize()
    {
        return new[]
        {
            (byte)BadConsolePacketType.HeartBeat,
        };
    }

    /// <summary>
    ///     Deserializes the Packet
    /// </summary>
    /// <param name="data">The Data Array</param>
    /// <returns>Bad Console Packet instance</returns>
    public new static BadConsoleHeartBeatPacket Deserialize(byte[] data)
    {
        if (data.Length != 1)
        {
            throw new BadNetworkConsoleException("Invalid HeartBeat Packet");
        }

        return Packet;
    }
}