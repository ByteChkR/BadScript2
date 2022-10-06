using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;
using BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client;

public class BadNetworkConsoleClient
{
    private readonly string m_Address;
    private readonly TcpClient m_Client;
    private readonly List<BadNetworkConsoleClientCommand> m_Commands = new List<BadNetworkConsoleClientCommand>();
    private readonly int m_Port;
    private bool m_ExitRequested;
    private Thread? m_ReadThread;

    public BadNetworkConsoleClient(TcpClient client)
    {
        m_Client = client;
    }

    public BadNetworkConsoleClient(string address, int port)
    {
        m_Address = address;
        m_Port = port;
        m_Client = new TcpClient();
        AddCommand(new BadNetworkConsoleClientDisconnectCommand(this));
        AddCommand(new BadNetworkConsoleClientListCommand(this));
    }

    public static int HeartBeatSendInterval { get; set; } = 3000;
    public static int ConsoleReadInputSleep { get; set; } = 100;
    public static int ConsoleWriteSleep { get; set; } = 100;
    public IEnumerable<BadNetworkConsoleClientCommand> Commands => m_Commands;

    public void AddCommand(BadNetworkConsoleClientCommand command)
    {
        m_Commands.Add(command);
    }

    public void Start()
    {
        m_ExitRequested = false;
        BadConsole.WriteLine($"[Console Client] Connecting to {m_Address}:{m_Port}");
        if (!m_Client.Connected)
        {
            m_Client.Connect(m_Address, m_Port);
        }

        BadConsole.WriteLine("[Console Client] Connected");


        m_ReadThread = new Thread(Read);
        m_ReadThread.Start();
        Write();
    }

    public void Stop()
    {
        m_ExitRequested = true;
    }

    private void ProcessPacket(BadConsolePacket packet)
    {
        if (packet is BadConsoleClearPacket)
        {
            Console.Clear();
        }
        else if (packet is BadConsoleDisconnectPacket)
        {
            m_ExitRequested = true;
        }
        else if (packet is BadConsoleHelloPacket hello)
        {
            HeartBeatSendInterval = hello.HeartBeatInterval;
        }
        else if (packet is BadConsoleWritePacket wp)
        {
            if (wp.IsWriteLine)
            {
                Console.WriteLine(wp.Message);
            }
            else
            {
                Console.Write(wp.Message);
            }
        }
        else if (packet is BadConsoleColorChangePacket cs)
        {
            if (cs.IsBackground)
            {
                Console.BackgroundColor = cs.Color;
            }
            else
            {
                Console.ForegroundColor = cs.Color;
            }
        }
        else
        {
            throw new Exception("Invalid Packet");
        }
    }

    private void Write()
    {
        while (!m_ExitRequested)
        {
            if (!m_Client!.Connected)
            {
                break;
            }

            if (m_Client.Available != 0)
            {
                byte[] len = new byte[sizeof(int)];
                NetworkStream stream = m_Client.GetStream();
                int read = stream.Read(len, 0, len.Length);
                if (read != len.Length)
                {
                    throw new Exception("Invalid Packet Size");
                }

                byte[] packet = new byte[BitConverter.ToInt32(len, 0)];
                read = stream.Read(packet, 0, packet.Length);

                if (read != packet.Length)
                {
                    throw new Exception("Invalid Packet");
                }

                ProcessPacket(BadConsolePacket.Deserialize(packet));
            }
            else
            {
                Thread.Sleep(ConsoleWriteSleep);
            }
        }

        if (m_Client!.Connected)
        {
            NetworkStream stream = m_Client.GetStream();
            List<byte> packetData = new List<byte>();
            byte[] packetBytes = BadConsoleDisconnectPacket.Packet.Serialize();
            packetData.AddRange(BitConverter.GetBytes(packetBytes.Length));
            packetData.AddRange(packetBytes);
            stream.Write(packetData.ToArray(), 0, packetData.Count());
            m_Client.Dispose();
        }
    }

    private void SendHeartBeat()
    {
        byte[] packetData = BadConsoleHeartBeatPacket.Packet.Serialize();
        List<byte> packet = new List<byte>();
        packet.AddRange(BitConverter.GetBytes(packetData.Length));
        packet.AddRange(packetData);
        m_Client!.GetStream().Write(packet.ToArray(), 0, packet.Count);
    }

    private void Read()
    {
        DateTime lastHeartBeat = DateTime.Now;
        while (!m_ExitRequested)
        {
            Task<string> task = Task.Run(Console.ReadLine);
            while (!m_ExitRequested && !task.IsCompleted)
            {
                if (lastHeartBeat + TimeSpan.FromMilliseconds(HeartBeatSendInterval) < DateTime.Now)
                {
                    SendHeartBeat();
                    lastHeartBeat = DateTime.Now;
                }

                Thread.Sleep(ConsoleReadInputSleep);
            }

            if (m_ExitRequested)
            {
                break;
            }

            string message = task.Result;


            if (message.StartsWith("client::"))
            {
                int idx = message.IndexOf(' ');
                if (idx == -1)
                {
                    idx = message.Length;
                }

                string cmdName = message.Substring(0, idx).Remove(0, "client::".Length);
                string args = message.Remove(0, idx).Trim();
                BadNetworkConsoleClientCommand? cmd = m_Commands.FirstOrDefault(x => x.Name == cmdName);
                if (cmd == null)
                {
                    BadConsole.WriteLine("Command not found: " + cmdName);
                }
                else
                {
                    cmd.Invoke(args);
                }

                continue;
            }

            lastHeartBeat = DateTime.Now;
            byte[] packetData = new BadConsoleReadPacket(message).Serialize();
            List<byte> packet = new List<byte>();
            packet.AddRange(BitConverter.GetBytes(packetData.Length));
            packet.AddRange(packetData);
            m_Client!.GetStream().Write(packet.ToArray(), 0, packet.Count);
        }

        m_ReadThread = null;
    }
}