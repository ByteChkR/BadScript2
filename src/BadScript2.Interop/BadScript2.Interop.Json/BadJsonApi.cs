using BadScript2.Runtime;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Settings;

namespace BadScript2.Interop.Json;

/// <summary>
///     Implements the "Json" Api
/// </summary>
[BadInteropApi("Json")]
internal partial class BadJsonApi
{
    [BadMethod(description: "Converts a JSON String to a BadObject")]
    [return: BadReturn("The Parsed Object")]
    private BadObject FromJson(BadExecutionContext ctx, [BadParameter(description: "The JSON String")] string str)
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
    }

    [BadMethod(description: "Converts a BadObject to a JSON String")]
    [return: BadReturn("The JSON String")]
    private string ToJson([BadParameter(description: "The Object to be converted.")] BadObject o)
    {
        return BadJson.ToJson(o);
    }

    protected override void AdditionalData(BadTable target)
    {
        target.SetProperty(
            "Settings",
            new BadSettingsObject(BadSettingsProvider.RootSettings),
            new BadPropertyInfo(BadSettingsObject.Prototype, true)
        );
    }
}