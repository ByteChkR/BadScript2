using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Task;

public class BadInteropRunnable : BadRunnable
{
    private BadObject m_ReturnValue = BadObject.Null;

    public BadInteropRunnable(IEnumerator<BadObject> enumerator)
    {
        Enumerator = enumerator;
    }

    public override IEnumerator<BadObject> Enumerator { get; }

    public void SetReturn(BadObject obj)
    {
        m_ReturnValue = obj;
    }

    public override BadObject GetReturn()
    {
        return m_ReturnValue;
    }
}