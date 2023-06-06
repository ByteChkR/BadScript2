using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Settings;

namespace BadScript2.Interop.Json;

/// <summary>
/// Implements the "Json" Api
/// </summary>
public class BadJsonApi : BadInteropApi
{
	/// <summary>
	/// Creates a new "Json" Api
	/// </summary>
	public BadJsonApi() : base("Json") { }

	protected override void LoadApi(BadTable target)
	{
		target.SetProperty("FromJson",
			new BadDynamicInteropFunction<string>("FromJson",
				(ctx, str) =>
				{
					try
					{
						return BadJson.FromJson(str);
					}
					catch (Exception e)
					{
						ctx.Scope.SetError(e.Message, null);
					}

					return BadObject.Null;
				},
				new BadFunctionParameter("str", false, true, false)));
		target.SetFunction<BadObject>("ToJson", o => BadJson.ToJson(o));
		target.SetProperty("Settings",
			new BadSettingsObject(BadSettingsProvider.RootSettings),
			new BadPropertyInfo(BadSettingsObject.Prototype));
	}
}
