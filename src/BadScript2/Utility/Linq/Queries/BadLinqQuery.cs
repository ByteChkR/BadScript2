using System.Collections;
using System.Globalization;
using System.Text;

using BadScript2.Utility.Linq.Queries.Commands;

namespace BadScript2.Utility.Linq.Queries;

/// <summary>
///     Implements the BadLinqQuery funcitonality.
/// </summary>
public static class BadLinqQuery
{
    private static readonly List<BadLinqQueryCommand> s_Commands = new List<BadLinqQueryCommand>();

    static BadLinqQuery()
    {
        RegisterCommand(new BadLinqQuerySelectCommand());
        RegisterCommand(new BadLinqQuerySelectManyCommand());
        RegisterCommand(new BadLinqQueryWhereCommand());
        RegisterCommand(new BadLinqQueryTakeCommand());
        RegisterCommand(new BadLinqQuerySkipCommand());
        RegisterCommand(new BadLinqQuerySkipLastCommand());
        RegisterCommand(new BadLinqQueryTakeLastCommand());
        RegisterCommand(new BadLinqQuerySkipWhileCommand());
        RegisterCommand(new BadLinqQueryTakeWhileCommand());
        RegisterCommand(new BadLinqQueryOrderByCommand());
        RegisterCommand(new BadLinqQueryOrderByDescendingCommand());
    }

    public static void RegisterCommand(BadLinqQueryCommand command)
    {
        s_Commands.Add(command);
    }

    public static IEnumerable Parse(string linqQuery, IEnumerable input)
    {
        IEnumerable current = input;

        while (!string.IsNullOrEmpty(linqQuery))
        {
            linqQuery = linqQuery.Trim();
            string command = linqQuery.Split(' ').First();
            linqQuery = linqQuery.Remove(0, command.Length);
            BadLinqQueryCommand cmd = s_Commands.First(
                x => x.Names.Any(
                    y =>
                        y.ToLower(CultureInfo.InvariantCulture) == command.ToLower(CultureInfo.InvariantCulture)
                )
            );

            if (cmd.HasArgument)
            {
                StringBuilder sb = new StringBuilder();

                while (!string.IsNullOrEmpty(linqQuery) &&
                       !s_Commands.Any(x => x.Names.Any(y => linqQuery.StartsWith(y))))
                {
                    sb.Append(linqQuery[0]);
                    linqQuery = linqQuery.Substring(1);
                }

                string arg = sb.ToString().Trim();

                if (string.IsNullOrWhiteSpace(arg) && !cmd.IsArgumentOptional)
                {
                    throw new Exception("Missing Argument for command " + cmd);
                }

                current = cmd.Run(new BadLinqQueryCommandData(current, arg));
            }
            else
            {
                current = cmd.Run(new BadLinqQueryCommandData(current));
            }
        }

        return current;
    }
}