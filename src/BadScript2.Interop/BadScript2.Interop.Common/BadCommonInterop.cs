using BadScript2.Interop.Common.Apis;
using BadScript2.Interop.Common.Extensions;
using BadScript2.Interop.Common.Task;
using BadScript2.Runtime.Interop;

namespace BadScript2.Interop.Common;

/// <summary>
///     Implements the Common Interop Wrapper
/// </summary>
public static class BadCommonInterop
{
	/// <summary>
	///     All Common Interop Apis
	/// </summary>
	private static readonly BadInteropApi[] s_CommonApis =
	{
		new BadConsoleApi(),
		new BadRuntimeApi(),
		new BadMathApi(),
		new BadOperatingSystemApi(),
		new BadXmlApi()
	};

	/// <summary>
	///     All Common Interop Apis
	/// </summary>
	public static IEnumerable<BadInteropApi> Apis => s_CommonApis;

	/// <summary>
	///     Adds all Common Interop Extensions to the BadScript Runtime
	/// </summary>
	public static void AddExtensions()
	{
		BadInteropExtension.AddExtension<BadObjectExtension>();
		BadInteropExtension.AddExtension<BadStringExtension>();
		BadInteropExtension.AddExtension<BadTableExtension>();
		BadInteropExtension.AddExtension<BadScopeExtension>();
		BadInteropExtension.AddExtension<BadArrayExtension>();
		BadInteropExtension.AddExtension<BadFunctionExtension>();
		BadInteropExtension.AddExtension<BadTypeSystemExtension>();
		BadInteropExtension.AddExtension<BadTaskExtensions>();
	}
}
