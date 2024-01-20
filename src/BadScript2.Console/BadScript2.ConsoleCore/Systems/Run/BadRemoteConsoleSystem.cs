using BadScript2.ConsoleAbstraction;
using BadScript2.ConsoleAbstraction.Implementations.Remote.Client;
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
    public BadRemoteConsoleSystem(BadRuntime runtime) : base(runtime) { }

    public override string Name => "remote";

    private IBadNetworkConsoleClientCommandParser CreateScriptParser(BadNetworkConsoleClient client)
    {
        return new ScriptCommandParser(Runtime, client);
    }

    protected override int Run(BadRemoteConsoleSystemSettings settings)
    {
        Func<BadNetworkConsoleClient, IBadNetworkConsoleClientCommandParser> parser = settings.UseScriptCommands ? CreateScriptParser : BadNetworkConsoleClient.DefaultParserFactory;
        BadNetworkConsoleClient client = new BadNetworkConsoleClient(settings.Host, settings.Port, parser);
        client.Start();

        return 0;
    }

    private class ScriptCommandParser : IBadNetworkConsoleClientCommandParser
    {
        private readonly BadNetworkConsoleClient m_Client;
        private readonly BadRuntime m_Runtime;
        private BadExecutionContext? m_Context;

        public ScriptCommandParser(BadRuntime runtime, BadNetworkConsoleClient client)
        {
            m_Runtime = runtime;
            m_Client = client;
        }

        public void ExecuteCommand(string command)
        {
            try
            {
                BadExecutionContext ctx = GetContext();
                IEnumerable<BadExpression> parsed = m_Runtime.Parse(command);
                foreach (BadObject o in ctx.Execute(parsed))
                {
                    //Do nothing
                }
            }
            catch (Exception e)
            {
                BadConsole.WriteLine(e.ToString());
            }
        }

        private BadExecutionContext GetContext()
        {
            return m_Context ??= CreateContext();
        }

        private BadExecutionContext CreateContext()
        {
            BadExecutionContext context = m_Runtime.CreateContext();
            context.Scope.SetFunction("disconnect", m_Client.Stop);
            context.Scope.SetFunction("exit", m_Client.Stop);
            context.Scope.SetFunction("clear", BadConsole.Clear);
            context.Scope.SetFunction("list", () => context.Scope.ToSafeString());
            context.Scope.SetFunction("reset", () => { m_Context = CreateContext(); });

            return context;
        }
    }
}