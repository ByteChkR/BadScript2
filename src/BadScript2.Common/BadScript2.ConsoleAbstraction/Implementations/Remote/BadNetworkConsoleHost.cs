using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using BadScript2.ConsoleAbstraction.Implementations.Remote.Packets;
namespace BadScript2.ConsoleAbstraction.Implementations.Remote;

/// <summary>
///     Implements a Host for the Remote Console
/// </summary>
public class BadNetworkConsoleHost : IBadConsole
{
    /// <summary>
    ///     Queue of Incoming Packets
    /// </summary>
    private readonly ConcurrentQueue<BadConsoleReadPacket> m_IncomingPackets =
        new ConcurrentQueue<BadConsoleReadPacket>();

    /// <summary>
    ///     The Used TCP Listener
    /// </summary>
    private readonly TcpListener? m_Listener;

    /// <summary>
    ///     Lock Object
    /// </summary>
    private readonly object m_Lock = new object();

    /// <summary>
    ///     The Outgoing Packet Queue
    /// </summary>
    private readonly ConcurrentQueue<BadConsolePacket> m_OutgoingPackets = new ConcurrentQueue<BadConsolePacket>();

    /// <summary>
    ///     The Background Color
    /// </summary>
    private ConsoleColor m_BackgroundColor = ConsoleColor.Black;

    /// <summary>
    ///     The Connected Client
    /// </summary>
    private TcpClient? m_Client;

    /// <summary>
    ///     If true the host will exit
    /// </summary>
    private bool m_ExitRequested;

    /// <summary>
    ///     The Foreground Color
    /// </summary>
    private ConsoleColor m_ForegroundColor = ConsoleColor.White;

    /// <summary>
    ///     The Message Thread
    /// </summary>
    private Thread? m_MessageThread;

    /// <summary>
    ///     Constructs a new Host from a TCP Listener
    /// </summary>
    /// <param name="listner">The TCP Listener</param>
    public BadNetworkConsoleHost(TcpListener listner)
    {
        m_Listener = listner;
    }

    /// <summary>
    ///     Constructs a new Host from a TCP Client
    /// </summary>
    /// <param name="client">The TCP Client</param>
    public BadNetworkConsoleHost(TcpClient client)
    {
        m_Client = client;
    }

    /// <summary>
    ///     Is true if a client is connected to this host
    /// </summary>
    public bool IsConnected => m_Client != null;

    /// <summary>
    ///     The Interval in which the Host will send Heartbeat Packets to the Client
    /// </summary>
    public static int HeartBeatInterval { get; set; } = 3000;

    /// <summary>
    ///     The Timeout after which the Host will disconnect the Client if no Heartbeat Packet was received
    /// </summary>
    public static int HeartBeatTimeOut { get; set; } = 5000;

    /// <summary>
    ///     The Time Interval that the Read Thread will sleep if no data is available
    /// </summary>
    public static int ReadSleepTimeout { get; set; } = 100;


    /// <summary>
    ///     The Time Interval that the Write Thread will sleep if no data is available
    /// </summary>
    public static int ReceiveSleepTimeout { get; set; } = 100;

    /// <summary>
    ///     The Time Interval that the Accept Thread will sleep if no data is available
    /// </summary>
    public static int AcceptSleepTimeout { get; set; } = 100;

    /// <inheritdoc />
    public void Write(string str)
    {
        m_OutgoingPackets.Enqueue(new BadConsoleWritePacket(false, str));
    }

    /// <inheritdoc />
    public void WriteLine(string str)
    {
        m_OutgoingPackets.Enqueue(new BadConsoleWritePacket(true, str));
    }

    /// <inheritdoc />
    public string ReadLine()
    {
        BadConsoleReadPacket ret;

        while (!m_IncomingPackets.TryDequeue(out ret))
        {
            Thread.Sleep(ReadSleepTimeout);
        }

        return ret.Message;
    }

    /// <inheritdoc />
    public Task<string> ReadLineAsync()
    {
        return Task.Run(ReadLine);
    }

    /// <inheritdoc />
    public void Clear()
    {
        m_OutgoingPackets.Enqueue(BadConsoleClearPacket.Packet);
    }

    /// <inheritdoc />
    public ConsoleColor ForegroundColor
    {
        get => m_ForegroundColor;
        set
        {
            m_ForegroundColor = value;
            m_OutgoingPackets.Enqueue(new BadConsoleColorChangePacket(false, value));
        }
    }

    /// <inheritdoc />
    public ConsoleColor BackgroundColor
    {
        get => m_BackgroundColor;
        set
        {
            m_BackgroundColor = value;
            m_OutgoingPackets.Enqueue(new BadConsoleColorChangePacket(true, value));
        }
    }

    /// <summary>
    ///     Starts the Host
    /// </summary>
    /// <exception cref="BadNetworkConsoleException">Gets raised if the Message Thread is already running</exception>
    public void Start()
    {
        if (m_MessageThread != null)
        {
            throw new BadNetworkConsoleException("Message Thread already running");
        }

        lock (m_Lock)
        {
            m_ExitRequested = false;
        }

        m_MessageThread = new Thread(MessageThread);
        m_MessageThread.Start();
    }

    /// <summary>
    ///     Stops the Host
    /// </summary>
    /// <exception cref="BadNetworkConsoleException">Gets raised if the Message thread is not running</exception>
    public void Stop()
    {
        if (m_MessageThread == null)
        {
            throw new BadNetworkConsoleException("Message Thread is not running");
        }

        lock (m_Lock)
        {
            m_ExitRequested = true;
        }
    }


    /// <summary>
    ///     Sends a Disconnect Packet to the Client
    /// </summary>
    public void Disconnect()
    {
        m_OutgoingPackets.Enqueue(BadConsoleDisconnectPacket.Packet);
    }

    /// <summary>
    ///     The Message Thread Loop
    /// </summary>
    /// <exception cref="BadNetworkConsoleException">gets raised if the packed could not be read</exception>
    private void MessageThread()
    {
        while (!m_ExitRequested)
        {
            if (m_Listener != null && m_Client is not { Connected: true })
            {
                m_Listener.Start();
                BadConsole.WriteLine($"[Console Host] Waiting for Connection on {m_Listener.LocalEndpoint}");
                bool accepted = false;
                m_Listener.BeginAcceptTcpClient(
                    ar =>
                    {
                        m_Client = m_Listener.EndAcceptTcpClient(ar);
                        accepted = true;
                    },
                    null
                );

                while (!accepted && !m_ExitRequested)
                {
                    Thread.Sleep(AcceptSleepTimeout);
                }

                m_Listener.Stop();
            }

            m_OutgoingPackets.Enqueue(new BadConsoleHelloPacket(HeartBeatInterval));
            DateTime lastHeartBeat = DateTime.Now;

            while (!m_ExitRequested && m_Client != null && m_Client!.Connected)
            {
                bool done = false;

                if (m_Client.Available != 0)
                {
                    done = true;
                    lastHeartBeat = DateTime.Now;
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

                    BadConsolePacket packetObj = BadConsolePacket.Deserialize(packet);

                    if (packetObj is BadConsoleReadPacket rp)
                    {
                        m_IncomingPackets.Enqueue(rp);
                    }
                    else if (packetObj is BadConsoleHeartBeatPacket)
                    {
                        //Ignore Packet
                    }
                    else if (packetObj is BadConsoleDisconnectPacket)
                    {
                        m_Client.Dispose();
                        m_Client = null;

                        break;
                    }
                    else
                    {
                        throw new BadNetworkConsoleException("Invalid Packet Type");
                    }
                }

                if (m_Client != null && m_OutgoingPackets.Count != 0)
                {
                    if (m_OutgoingPackets.TryDequeue(out BadConsolePacket packet))
                    {
                        done = true;
                        NetworkStream stream = m_Client.GetStream();
                        List<byte> packetData = new List<byte>();
                        byte[] packetBytes = packet.Serialize();
                        packetData.AddRange(BitConverter.GetBytes(packetBytes.Length));
                        packetData.AddRange(packetBytes);
                        stream.Write(packetData.ToArray(), 0, packetData.Count);
                    }
                }

                if (done)
                {
                    continue;
                }

                if (lastHeartBeat + TimeSpan.FromMilliseconds(HeartBeatTimeOut) < DateTime.Now)
                {
                    m_Client?.Dispose();
                }

                Thread.Sleep(ReceiveSleepTimeout);
            }
        }

        if (m_Client != null && m_Client!.Connected)
        {
            NetworkStream stream = m_Client.GetStream();
            List<byte> packetData = new List<byte>();
            byte[] packetBytes = BadConsoleDisconnectPacket.Packet.Serialize();
            packetData.AddRange(BitConverter.GetBytes(packetBytes.Length));
            packetData.AddRange(packetBytes);
            stream.Write(packetData.ToArray(), 0, packetData.Count);
            m_Client.Dispose();
            m_Client = null;
        }

        m_MessageThread = null;
    }
}