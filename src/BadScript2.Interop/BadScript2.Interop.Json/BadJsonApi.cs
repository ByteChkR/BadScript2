using System.Runtime.ExceptionServices;

using BadScript2.Runtime;
using BadScript2.Runtime.Error;
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
    /// <summary>
    /// Converts a BadObject to a JSON String
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="str">The JSON String</param>
    /// <returns>The Parsed Object</returns>
    /// <exception cref="BadRuntimeErrorException">Gets thrown if the JSON is invalid</exception>
    [BadMethod(description: "Converts a JSON String to a BadObject")]
    [return: BadReturn("The Parsed Object")]
    private BadObject FromJson(BadExecutionContext ctx, [BadParameter(description: "The JSON String")] string str)
    {
        try
        {
            return BadJson.FromJson(str);
        }
        catch (BadRuntimeErrorException e)
        {
            ExceptionDispatchInfo.Capture(e)
                                 .Throw();
        }
        catch (Exception e)
        {
            throw new BadRuntimeErrorException(BadRuntimeError.FromException(e, ctx.Scope.GetStackTrace()));
        }

        return BadObject.Null;
    }

    /// <summary>
    /// Converts a BadObject to a JSON String
    /// </summary>
    /// <param name="o">The Object to be converted</param>
    /// <returns>The JSON String</returns>
    [BadMethod(description: "Converts a BadObject to a JSON String")]
    [return: BadReturn("The JSON String")]
    private string ToJson([BadParameter(description: "The Object to be converted.")] BadObject o)
    {
        return BadJson.ToJson(o);
    }
    
    /// <summary>
    /// Converts a BadObject to a YAML String
    /// </summary>
    /// <param name="o">The Object to be converted</param>
    /// <returns>The YAML String</returns>
    [BadMethod(description: "Converts a BadObject to a YAML String")]
    [return: BadReturn("The YAML String")]
    private string ToYaml([BadParameter(description: "The Object to be converted.")] BadObject o)
    {
        return BadJson.ToYaml(o);
    }
    
    /// <summary>
    /// Converts a YAML String to a BadObject
    /// </summary>
    /// <param name="ctx">The Execution Context</param>
    /// <param name="str">The YAML String</param>
    /// <returns>The Parsed Object</returns>
    /// <exception cref="BadRuntimeErrorException">Gets thrown if the YAML is invalid</exception>
    [BadMethod(description: "Converts a YAML String to a BadObject")]
    [return: BadReturn("The Parsed Object")]
    private BadObject FromYaml(BadExecutionContext ctx, [BadParameter(description: "The YAML String")] string str)
    {
        try
        {
            return BadJson.FromYaml(str);
        }
        catch (BadRuntimeErrorException e)
        {
            ExceptionDispatchInfo.Capture(e)
                                 .Throw();
        }
        catch (Exception e)
        {
            throw new BadRuntimeErrorException(BadRuntimeError.FromException(e, ctx.Scope.GetStackTrace()));
        }

        return BadObject.Null;
    }

    /// <inheritdoc />
    protected override void AdditionalData(BadTable target)
    {
        target.SetProperty("Settings",
                           new BadSettingsObject(BadSettingsProvider.RootSettings),
                           new BadPropertyInfo(BadSettingsObject.Prototype, true)
                          );
    }
}