using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Objects;
using BadScript2.Settings;

namespace BadScript2.Interop.Json
{
    public class BadJsonApi : BadInteropApi
    {
        public BadJsonApi() : base("Json") { }

        public override void Load(BadTable target)
        {
            target.SetFunction<string>("FromJson", BadJson.FromJson);
            target.SetFunction<BadObject>("ToJson", o => BadJson.ToJson(o));
            target.SetProperty(
                "Settings",
                new BadSettingsObject(BadSettingsProvider.RootSettings),
                new BadPropertyInfo(BadSettingsObject.Prototype)
            );
        }
    }
}