using System;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Packets
{
    public class BadConsoleColorChangePacket : BadConsolePacket
    {
        public readonly ConsoleColor Color;
        public readonly bool IsBackground;

        public BadConsoleColorChangePacket(bool isBackground, ConsoleColor color)
        {
            IsBackground = isBackground;
            Color = color;
        }

        public new static BadConsoleColorChangePacket Deserialize(byte[] data)
        {
            bool isBackground = data[1] == 1;
            ConsoleColor color = (ConsoleColor)data[2];

            return new BadConsoleColorChangePacket(isBackground, color);
        }

        public override byte[] Serialize()
        {
            return new[] { (byte)BadConsolePacketType.Color, (byte)(IsBackground ? 1 : 0), (byte)Color };
        }
    }
}