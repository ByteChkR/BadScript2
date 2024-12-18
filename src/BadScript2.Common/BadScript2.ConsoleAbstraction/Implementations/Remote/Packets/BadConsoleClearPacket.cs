/// <summary>
/// Contains the network packets for the remote console
/// </summary>

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

/// <summary>
///     Gets used to clear the console on the client
/// </summary>
public class BadConsoleClearPacket : BadConsolePacket
{
	/// <summary>
	///     Static Instance of this Packet
	/// </summary>
	public static readonly BadConsoleClearPacket Packet = new BadConsoleClearPacket();

	/// <summary>
	///     Private Constructor
	/// </summary>
	private BadConsoleClearPacket() { }

	/// <summary>
	///     Deserializes the Packet
	/// </summary>
	/// <param name="data">The Data Array</param>
	/// <returns>Bad Console Packet instance</returns>
	public new static BadConsoleClearPacket Deserialize(byte[] data)
    {
        if (data.Length != 1)
        {
            throw new BadNetworkConsoleException("Invalid Clear Packet");
        }

        return Packet;
    }

    /// <inheritdoc />
    public override byte[] Serialize()
    {
        return new[] { (byte)BadConsolePacketType.Clear };
    }
}