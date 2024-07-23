using System;
using System.Collections.Generic;
using System.Linq;

using BadScript2.ConsoleAbstraction.Implementations.Remote.Client.Commands;

/// <summary>
/// Contains the Console Client Implementation for the Remote Console Abstraction over TCP
/// </summary>
namespace BadScript2.ConsoleAbstraction.Implementations.Remote.Client;

/// <summary>
///     The Default Command Parser for the Remote Console Client
/// </summary>
public class BadDefaultNetworkClientCommandParser : IBadNetworkConsoleClientCommandParser
{
    /// <summary>
    ///     The Client
    /// </summary>
    private readonly BadNetworkConsoleClient m_Client;

    /// <summary>
    ///     The Command List
    /// </summary>
    private readonly List<BadNetworkConsoleClientCommand> m_Commands = new List<BadNetworkConsoleClientCommand>();

    /// <summary>
    ///     Creates a new Command Parser
    /// </summary>
    /// <param name="client">The Client</param>
    public BadDefaultNetworkClientCommandParser(BadNetworkConsoleClient client)
    {
        m_Client = client;
    }

    /// <summary>
    ///     The Commands
    /// </summary>
    public IEnumerable<BadNetworkConsoleClientCommand> Commands => m_Commands;


    /// <inheritdoc />
    public void ExecuteCommand(string command)
    {
        string[] parts = command.Split(
            new[]
            {
                ' ',
            },
            StringSplitOptions.RemoveEmptyEntries
        );

        if (parts.Length == 0)
        {
            return;
        }

        string name = parts[0];
        foreach (BadNetworkConsoleClientCommand? cmd in m_Commands)
        {
            if (cmd.Name == name)
            {
                cmd.Invoke(parts.Skip(1).ToArray());

                return;
            }
        }

        BadConsole.WriteLine("Unknown Command: " + name);
    }

    /// <summary>
    ///     Adds a Command to the Command List
    /// </summary>
    /// <param name="command">The Command to be added</param>
    public void AddCommand(Func<BadNetworkConsoleClient, BadNetworkConsoleClientCommand> command)
    {
        m_Commands.Add(command(m_Client));
    }
}