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
[BadInteropApi("Xml")]
internal partial class BadXmlApi
{
	/// <summary>
	///     Loads an XML Document from a string
	/// </summary>
	/// <param name="xml">Xml String</param>
	/// <returns>The XmlDocument wrapped in a Reflected Object</returns>
	[BadMethod(description: "Loads an XML Document from a string")]
	[return: BadReturn("XmlDocument")]
	private BadObject Load(string xml)
	{
		
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(xml);

		return new BadReflectedObject(doc);
	}

}