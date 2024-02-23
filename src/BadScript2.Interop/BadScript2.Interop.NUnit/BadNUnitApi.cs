using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;

///<summary>
///	Contains NUnit Extensions and APIs for the BadScript2 Runtime
/// </summary>
namespace BadScript2.Interop.NUnit;

[BadInteropApi("NUnit")]
internal partial class BadNUnitApi
{
    protected override void AdditionalData(BadTable target)
    {
        BadTable assert = new BadTable();
        new BadNUnitAssertApi().LoadRawApi(assert);
        target.SetProperty("Assert", assert, new BadPropertyInfo(BadTable.Prototype, true));
    }
}