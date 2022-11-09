using BadScript2.Runtime.Objects;

namespace BadScript2.Interop.Common.Task;

public class BadInteropRunnable : BadRunnable
{
    private BadObject m_ReturnValue = BadObject.Null;


    public BadInteropRunnable(IEnumerator<BadObject> enumerator, bool setLastAsReturn = false)
    {
        Enumerator = setLastAsReturn ? CreateEnumerator(enumerator) : enumerator;
    }

    public override IEnumerator<BadObject> Enumerator { get; }

    private IEnumerator<BadObject> CreateEnumerator(IEnumerator<BadObject> enumerator)
    {
        BadObject last = enumerator.Current ?? BadObject.Null;
        while (enumerator.MoveNext())
        {
            last = enumerator.Current ?? BadObject.Null;
            yield return last;
        }

        SetReturn(last);
    }

    public void SetReturn(BadObject obj)
    {
        m_ReturnValue = obj;
    }

    public override BadObject GetReturn()
    {
        return m_ReturnValue;
    }
}