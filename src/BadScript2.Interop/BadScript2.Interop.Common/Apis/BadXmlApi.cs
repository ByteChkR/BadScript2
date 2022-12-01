using System.Xml;

using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Apis;

public class BadXmlApi : BadInteropApi
{
    public BadXmlApi() : base("Xml") { }

    public static BadObject LoadXml(string s)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(s);


        return new BadReflectedObject(doc);
    }

    public override void Load(BadTable target)
    {
        target.SetFunction<string>("Load", LoadXml);
    }
}