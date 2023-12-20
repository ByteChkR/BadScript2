using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop;

/// <summary>
///     Implements an Interop API for the BS2 Language
/// </summary>
public abstract class BadInteropApi
{
	/// <summary>
	///     Creates a new Interop API
	/// </summary>
	/// <param name="name">Name of the API</param>
	protected BadInteropApi(string name)
	{
		Name = name;
	}

	/// <summary>
	///     Name of the API
	/// </summary>
	public string Name { get; }

	public virtual Version Version => GetType().Assembly.GetName().Version;

	/// <summary>
	///     Loads the API into the given Table
	/// </summary>
	/// <param name="target">Table Target</param>
	protected abstract void LoadApi(BadTable target);

	public void Load(BadTable table)
	{
		BadTable info = new BadTable();
		info.SetProperty("Name", Name, new BadPropertyInfo(BadNativeClassBuilder.GetNative("string"), true));
		info.SetProperty("Version", Version.ToString(), new BadPropertyInfo(BadNativeClassBuilder.GetNative("string"), true));
		info.SetProperty("AssemblyName", GetType().Assembly.GetName().Name, new BadPropertyInfo(BadNativeClassBuilder.GetNative("string"), true));

		if (!table.HasProperty("Info"))
		{
			table.SetProperty("Info", info, new BadPropertyInfo(BadNativeClassBuilder.GetNative("Table"), true));
		}

		LoadApi(table);
	}
}
