using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;
using BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;

namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client;

/// <summary>
///     Implements the Client for the Remote Console
/// </summary>
public class BadNetworkConsoleClient
{
    /// <summary>
    ///     The Address of the Remote Console
    /// </summary>
    private readonly string m_Address;

    /// <summary>
    ///     The TCP Client
    /// </summary>
    private readonly TcpClient m_Client;

    /// <summary>
    ///     The Parser
    /// </summary>
    private readonly IBadNetworkConsoleClientCommandParser m_Parser;


    /// <summary>
    ///     The Port of the Remote Console
    /// </summary>
    private readonly int m_Port;

    /// <summary>
    ///     If true the client will exit
    /// </summary>
    private bool m_ExitRequested;

    /// <summary>
    ///     The Read Thread
    /// </summary>
    private Thread? m_ReadThread;

    /// <summary>
    ///     Creates a new Client from the given TcpClient
    /// </summary>
    /// <param name="client">The TCP CLient</param>
    /// <param name="parserFactory">The Parser Factory</param>
    public BadNetworkConsoleClient(TcpClient client,
                                   Func<BadNetworkConsoleClient, IBadNetworkConsoleClientCommandParser> parserFactory)
    {
        m_Client = client;
        m_Address = "";
        m_Port = -1;
        m_Parser = parserFactory(this);
    }

    /// <summary>
    ///     Creates a new Client from the given TcpClient
    /// </summary>
    /// <param name="client">The TCP CLient</param>
    public BadNetworkConsoleClient(TcpClient client) : this(client, DefaultParserFactory) { }

    /// <summary>
    ///     Creates a new Client from the given Address and Port
    /// </summary>
    /// <param name="address">The Address of the Remote Console</param>
    /// <param name="port">The Port of the Remote Console</param>
    public BadNetworkConsoleClient(string address, int port) : this(address, port, DefaultParserFactory) { }

    /// <summary>
    ///     Creates a new Client from the given Address and Port
    /// </summary>
    /// <param name="address">The Address of the Remote Console</param>
    /// <param name="port">The Port of the Remote Console</param>
    /// <param name="parserFactory">The Parser Factory</param>
    public BadNetworkConsoleClient(string address,
                                   int port,
                                   Func<BadNetworkConsoleClient, IBadNetworkConsoleClientCommandParser> parserFactory)
    {
        m_Address = address;
        m_Port = port;
        m_Client = new TcpClient();
        m_Parser = parserFactory(this);
    }

    /// <summary>
    ///     The Interval in which the Heartbeat is sent
    /// </summary>
    public static int HeartBeatSendInterval { get; set; } = 3000;

    /// <summary>
    ///     The Interval in which packets are read from the server
    /// </summary>
    public static int ConsoleReadInputSleep { get; set; } = 100;

    /// <summary>
    ///     The Interval in which packets are sent to the server
    /// </summary>
    public static int ConsoleWriteSleep { get; set; } = 100;

    /// <summary>
    ///     The Default Parser Factory
    /// </summary>
    /// <param name="client">The Client</param>
    /// <returns>The Parser</returns>
    public static IBadNetworkConsoleClientCommandParser DefaultParserFactory(BadNetworkConsoleClient client)
    {
        BadDefaultNetworkClientCommandParser parser = new BadDefaultNetworkClientCommandParser(client);
        parser.AddCommand(c => new BadNetworkConsoleClientDisconnectCommand(c));
        parser.AddCommand(_ => new BadNetworkConsoleClientListCommand(parser));

        return parser;
    }


    /// <summary>
    ///     Starts the Client
    /// </summary>
    public void Start()
    {
        m_ExitRequested = false;
        BadConsole.WriteLine($"[Console Client] Connecting to {m_Address}:{m_Port}");

        if (!m_Client.Connected)
        {
            m_Client.Connect(m_Address, m_Port);
        }

        BadConsole.WriteLine("[Console Client] Connected");

        m_ReadThread = new Thread(Write);
        m_ReadThread.Start();
        Read();
    }

    /// <summary>
    ///     Stops the Client
    /// </summary>
    public void Stop()
    {
        m_ExitRequested = true;
    }

    /// <summary>
    ///     Processes the given Packet
    /// </summary>
    /// <param name="packet">The Packet</param>
    /// <exception cref="BadNetworkConsoleException">Gets raised if the packet could not be processed</exception>
    private void ProcessPacket(BadConsolePacket packet)
    {
        switch (packet)
        {
            case BadConsoleClearPacket:
                Console.Clear();

                break;
            case BadConsoleDisconnectPacket:
                m_ExitRequested = true;

                break;
            case BadConsoleHelloPacket hello:
                HeartBeatSendInterval = hello.HeartBeatInterval;

                break;
            case BadConsoleWritePacket { IsWriteLine: true } wp:
                Console.WriteLine(wp.Message);

                break;
            case BadConsoleWritePacket wp:
                Console.Write(wp.Message);

                break;
            case BadConsoleColorChangePacket { IsBackground: true } cs:
                Console.BackgroundColor = cs.Color;

                break;
            case BadConsoleColorChangePacket cs:
                Console.ForegroundColor = cs.Color;

                break;
            default:
                throw new BadNetworkConsoleException("Invalid Packet");
        }
    }

    /// <summary>
    ///     The Read Thread
    /// </summary>
    /// <exception cref="BadNetworkConsoleException">Gets raised if the incoming packets could not be read</exception>
    private void Read()
    {
        while (!m_ExitRequested)
        {
            if (!m_Client.Connected)
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
                    throw new BadNetworkConsoleException("Invalid Packet Size");
                }

                byte[] packet = new byte[BitConverter.ToInt32(len, 0)];
                read = stream.Read(packet, 0, packet.Length);

                if (read != packet.Length)
                {
                    throw new BadNetworkConsoleException("Invalid Packet");
                }

                ProcessPacket(BadConsolePacket.Deserialize(packet));
            }
            else
            {
                Thread.Sleep(ConsoleWriteSleep);
            }
        }

        if (!m_Client.Connected)
        {
            return;
        }

        NetworkStream str = m_Client.GetStream();
        List<byte> packetData = new List<byte>();
        byte[] packetBytes = BadConsoleDisconnectPacket.Packet.Serialize();
        packetData.AddRange(BitConverter.GetBytes(packetBytes.Length));
        packetData.AddRange(packetBytes);
        str.Write(packetData.ToArray(), 0, packetData.Count);
        m_Client.Dispose();
    }

    /// <summary>
    ///     Sends a Heartbeat to the Server
    /// </summary>
    private void SendHeartBeat()
    {
        byte[] packetData = BadConsoleHeartBeatPacket.Packet.Serialize();
        List<byte> packet = new List<byte>();
        packet.AddRange(BitConverter.GetBytes(packetData.Length));
        packet.AddRange(packetData);

        m_Client.GetStream()
                .Write(packet.ToArray(), 0, packet.Count);
    }

    /// <summary>
    ///     The Write Thread
    /// </summary>
    private void Write()
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

            string message = task.Result ?? "client::disconnect";

            if (message.StartsWith("client::"))
            {
                string cmd = message.Substring("client::".Length);
                m_Parser.ExecuteCommand(cmd);

                continue;
            }

            lastHeartBeat = DateTime.Now;
            byte[] packetData = new BadConsoleReadPacket(message).Serialize();
            List<byte> packet = new List<byte>();
            packet.AddRange(BitConverter.GetBytes(packetData.Length));
            packet.AddRange(packetData);

            m_Client.GetStream()
                    .Write(packet.ToArray(), 0, packet.Count);
        }

        m_ReadThread = null;
    }
}