using System.Text;

namespace BadScript2.WebEditor.Shared.Commandline
{
    public class BadHelpCommand : BadConsoleCommand
    {
        private readonly Func<IEnumerable<BadConsoleCommand>> m_GetCommands;

        public BadHelpCommand(Func<IEnumerable<BadConsoleCommand>> commandFunc) : base(
            "help",
            "Lists all commands including their description",
            new[] { "h", "?" },
            new[] { "(optional) searchTerm" }
        )
        {
            m_GetCommands = commandFunc;
        }

        public override string Execute(string args)
        {
            StringBuilder sb = new StringBuilder("Available Commands:\n");
            foreach (BadConsoleCommand cmd in m_GetCommands())
            {
                if (string.IsNullOrEmpty(args) || cmd.Name.StartsWith(args) || cmd.Aliases.Any(x => x.StartsWith(args)))
                {
                    sb.AppendLine("\t" + cmd.Name + " - " + cmd.Description);
                    sb.AppendLine("\t\tAliases: " + string.Join(", ", cmd.Aliases));
                    sb.AppendLine("\t\tArguments: " + string.Join("\n", cmd.Arguments));
                }
            }

            return sb.ToString();
        }
    }
}