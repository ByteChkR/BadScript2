using System.Xml;

using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Interop.Functions.Extensions;
using BadScript2.Runtime.Interop.Reflection.Objects;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Interop.Common.Apis;

/// <summary>
///     Implements the "Xml" API
/// </summary>
public class BadXmlApi : BadInteropApi
{
	/// <summary>
	///     Constructs a new Xml API Instance
	/// </summary>
	public BadXmlApi() : base("Xml") { }

	/// <summary>
	///     Loads an XML Document from a string
	/// </summary>
	/// <param name="s">Xml String</param>
	/// <returns>The XmlDocument wrapped in a Reflected Object</returns>
	public static BadObject LoadXml(string s)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(s);

        return new BadReflectedObject(doc);
    }

    protected override void LoadApi(BadTable target)
    {
        target.SetFunction<string>("Load", LoadXml, BadAnyPrototype.Instance);
    }
}