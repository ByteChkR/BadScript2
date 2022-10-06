using System;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets
{
    public abstract class BadConsolePacket
    {
        public static BadConsolePacket Deserialize(byte[] data)
        {
            BadConsolePacketType type = (BadConsolePacketType)data[0];
            switch (type)
            {
                case BadConsolePacketType.Write:
                    return BadConsoleWritePacket.Deserialize(data);
                case BadConsolePacketType.Read:
                    return BadConsoleReadPacket.Deserialize(data);
                case BadConsolePacketType.Color:
                    return BadConsoleColorChangePacket.Deserialize(data);
                case BadConsolePacketType.Clear:
                    return BadConsoleClearPacket.Deserialize(data);
                case BadConsolePacketType.Disconnect:
                    return BadConsoleDisconnectPacket.Deserialize(data);
                case BadConsolePacketType.HeartBeat:
                    return BadConsoleHeartBeatPacket.Deserialize(data);
                case BadConsolePacketType.Hello:
                    return BadConsoleHelloPacket.Deserialize(data);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract byte[] Serialize();
    }
}