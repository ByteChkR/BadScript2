using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.NetHost;

/// <summary>
///     Implements the "NetHost" Api
/// </summary>
public class BadNetHostApi : BadInteropApi
{
	/// <summary>
	///     Public Constructor
	/// </summary>
	public BadNetHostApi() : base("NetHost") { }


	protected override void LoadApi(BadTable target)
	{
		target.SetFunction<BadArray>("Create",
			p =>
			{
				string[] prefixes = new string[p.InnerArray.Count];

				for (int i = 0; i < prefixes.Length; i++)
				{
					if (p.InnerArray[i] is not IBadString str)
					{
						throw new BadRuntimeException("Invalid Prefix");
					}

					prefixes[i] = str.Value;
				}

				BadNetHost host = new BadNetHost(prefixes);
				BadTable table = new BadTable();
				table.SetFunction("Start", host.Start);
				table.SetFunction("Stop", host.Stop);
				table.SetFunction("Close", host.Close);
				table.SetFunction("Abort", host.Abort);
				table.SetFunction("AcceptClient", host.AcceptClient, BadTask.Prototype);

				return table;
			},
			BadNativeClassBuilder.GetNative("Table"));
	}
}
