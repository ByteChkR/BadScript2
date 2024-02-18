using BadScript2.ConsoleAbstraction;
using BadScript2.ConsoleAbstraction.Implementations.Remote.Client;
using BadScript2.IO;
using BadScript2.Parser.Expressions;
using BadScript2.Runtime;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.ConsoleCore.Systems.Run;

/// <summary>
///     Connects to a remote console
/// </summary>
public class BadRemoteConsoleSystem : BadConsoleSystem<BadRemoteConsoleSystemSettings>
{
    /// <summary>
    /// Creates a new BadRemoteConsoleSystem instance
    /// </summary>
    /// <param name="runtime">The Runtime to use</param>
    public BadRemoteConsoleSystem(BadRuntime runtime) : base(runtime) { }

    /// <inheritdoc/>
    public override string Name => "remote";

    /// <summary>
    /// Creates a new Script Command Parser
    /// </summary>
    /// <param name="client">The Client to use</param>
    /// <returns>The new Parser</returns>
    private IBadNetworkConsoleClientCommandParser CreateScriptParser(BadNetworkConsoleClient client)
    {
        return new BadScriptCommandParser(Runtime, client);
    }

    /// <inheritdoc/>
    protected override int Run(BadRemoteConsoleSystemSettings settings)
    {
        Func<BadNetworkConsoleClient, IBadNetworkConsoleClientCommandParser> parser = settings.UseScriptCommands ? CreateScriptParser : BadNetworkConsoleClient.DefaultParserFactory;
        BadNetworkConsoleClient client = new BadNetworkConsoleClient(settings.Host, settings.Port, parser);
        client.Start();

        return 0;
    }

    /// <summary>
    /// Script Command Parser
    /// </summary>
    private class BadScriptCommandParser : IBadNetworkConsoleClientCommandParser
    {
        /// <summary>
        /// The Client to use
        /// </summary>
        private readonly BadNetworkConsoleClient m_Client;
        /// <summary>
        /// The Runtime to use
        /// </summary>
        private readonly BadRuntime m_Runtime;
        /// <summary>
        /// The Current Context
        /// </summary>
        private BadExecutionContext? m_Context;

        /// <summary>
        /// Creates a new ScriptCommandParser instance
        /// </summary>
        /// <param name="runtime">The Runtime to use</param>
        /// <param name="client">The Client to use</param>
        public BadScriptCommandParser(BadRuntime runtime, BadNetworkConsoleClient client)
        {
            m_Runtime = runtime;
            m_Client = client;
        }

        
        /// <inheritdoc/>
        public void ExecuteCommand(string command)
        {
            try
            {
                BadExecutionContext ctx = GetContext();
                IEnumerable<BadExpression> parsed = m_Runtime.Parse(command);
                foreach (BadObject _ in ctx.Execute(parsed))
                {
                    //Do nothing
                }
            }
            catch (Exception e)
            {
                BadConsole.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Returns the Current Context
        /// </summary>
        /// <returns>The Current Context</returns>
        private BadExecutionContext GetContext()
        {
            return m_Context ??= CreateContext();
        }

        /// <summary>
        /// Creates a new Context
        /// </summary>
        /// <returns>The new Context</returns>
        private BadExecutionContext CreateContext()
        {
            BadExecutionContext context = m_Runtime.CreateContext(BadFileSystem.Instance.GetCurrentDirectory());
            BadExecutionContext ctx = new BadExecutionContext(context.Scope.CreateChild("ClientCommands", null, null));
            BadTable table = ctx.Scope.GetTable();
            table.SetFunction("disconnect", m_Client.Stop);
            table.SetFunction("exit", m_Client.Stop);
            table.SetFunction("clear", BadConsole.Clear);
            table.SetFunction("list", () => BadConsole.WriteLine(table.ToSafeString()));
            table.SetFunction("reset", () => { m_Context = CreateContext(); });

            return ctx;
        }
    }
}