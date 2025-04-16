using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.NetHost;

/// <summary>
///     Implements the "NetHost" Api
/// </summary>
[BadInteropApi("NetHost")]
internal partial class BadNetHostApi
{
    /// <summary>
    /// Creates a new NetHost Instance
    /// </summary>
    /// <param name="prefixes">Array of string prefixes</param>
    /// <returns>The NetHost Instance</returns>
    [BadMethod(description: "Creates a new NetHost Instance")]
    [return: BadReturn("The NetHost Instance")]
    private BadTable Create([BadParameter(description: "Array of string prefixes")] string[] prefixes)
    {
        BadNetHost host = new BadNetHost(prefixes);
        BadTable table = new BadTable();
        table.SetFunction("Start", host.Start);
        table.SetFunction("Stop", host.Stop);
        table.SetFunction("Close", host.Close);
        table.SetFunction("Abort", host.Abort);
        table.SetFunction("AcceptClient", host.AcceptClient, BadTask.Prototype);

        return table;
    }
}