using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

using BadScript2.ConsoleAbstraction.Implementations.Remote;
using BadScript2.Runtime.Objects;

namespace BadScript2.VirtualMachine.Managing;

public class BadVirtualMachineSession
{
    private readonly TcpClient m_Client;
    private readonly BadVirtualMachineService m_Service;
    private IEnumerator? m_Run;
    private BadVirtualMachineUser? m_User;

    public BadVirtualMachineSession(TcpClient client, BadVirtualMachineService service)
    {
        m_Client = client;
        m_Service = service;
    }

    private bool IsLoggedIn => m_User != null;

    private (string user, string pass, string vm) ReadLoginPacket()
    {
        byte[] lenBuf = new byte[sizeof(int)];
        m_Client.GetStream().Read(lenBuf, 0, sizeof(int));
        int userLen = BitConverter.ToInt32(lenBuf, 0);
        byte[] userBuf = new byte[userLen];
        m_Client.GetStream().Read(userBuf, 0, userLen);
        string user = Encoding.UTF8.GetString(userBuf);
        m_Client.GetStream().Read(lenBuf, 0, sizeof(int));
        int passLen = BitConverter.ToInt32(lenBuf, 0);
        byte[] passBuf = new byte[passLen];
        m_Client.GetStream().Read(passBuf, 0, passLen);
        string pass = Encoding.UTF8.GetString(passBuf);
        m_Client.GetStream().Read(lenBuf, 0, sizeof(int));
        int vmLen = BitConverter.ToInt32(lenBuf, 0);
        byte[] vmBuf = new byte[vmLen];
        m_Client.GetStream().Read(vmBuf, 0, vmLen);
        string vm = Encoding.UTF8.GetString(vmBuf);

        return (user, pass, vm);
    }

    public bool Process()
    {
        if (!m_Client.Connected)
        {
            return false;
        }

        if (!IsLoggedIn)
        {
            if (m_Client.Connected && m_Client.Available > 0)
            {
                (string user, string pass, string vm) loginInfo = ReadLoginPacket();
                BadVirtualMachineUser? user = m_Service.Authenticate(loginInfo.user, loginInfo.pass);
                if (user == null)
                {
                    m_Client.Dispose();

                    return false;
                }

                m_User = user;
                BadVirtualMachineManager manager = m_Service.GetManager(user);
                BadVirtualMachineInfo? machineInfo = manager.GetMachineInfo(loginInfo.vm);
                if (machineInfo == null)
                {
                    m_Client.Dispose();

                    return false;
                }

                m_Run = Run(new BadVirtualMachine(machineInfo)).GetEnumerator();
            }
        }
        else
        {
            if (m_Run == null)
            {
                return false;
            }
            
            if (!m_Run.MoveNext())
            {
                m_Client.Dispose();

                return false;
            }
        }

        return true;
    }

    private IEnumerable Run(BadVirtualMachine machine)
    {
        BadNetworkConsoleHost host = new BadNetworkConsoleHost(m_Client);
        IEnumerator<BadObject> e = machine.Execute(host).GetEnumerator();
        IEnumerator eh = host.MessageRoutine().GetEnumerator();
        while (e.MoveNext())
        {
            eh.MoveNext();
            yield return BadObject.Null;
        }
        
        host.Disconnect();
        while (eh.MoveNext())
        {
            //Wait for console host to close
        }
        
        machine.SaveState();
    }
}