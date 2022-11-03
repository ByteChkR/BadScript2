using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Settings;

namespace BadScript2.Interop.Json
{
    public class BadJsonApi : BadInteropApi
    {
        public BadJsonApi() : base("Json") { }

        public override void Load(BadTable target)
        {
            target.SetProperty("FromJson", new BadDynamicInteropFunction<string>("FromJson",
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
                }, new BadFunctionParameter("str", false, true, false)));
            target.SetFunction<BadObject>("ToJson", o => BadJson.ToJson(o));
            target.SetProperty(
                "Settings",
                new BadSettingsObject(BadSettingsProvider.RootSettings),
                new BadPropertyInfo(BadSettingsObject.Prototype)
            );
        }
    }
}