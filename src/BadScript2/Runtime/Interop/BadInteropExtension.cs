namespace BadScript2.Runtime.Interop;

/// <summary>
///     Public Extension API for the BS2 Runtime
/// </summary>
public abstract class BadInteropExtension
{
	internal void InnerAddExtensions(BadInteropExtensionProvider provider)
	{
		AddExtensions(provider);
	}

	/// <summary>
	///     Adds extensions to the list of registered extensions
	/// </summary>
	protected abstract void AddExtensions(BadInteropExtensionProvider provider);
}
