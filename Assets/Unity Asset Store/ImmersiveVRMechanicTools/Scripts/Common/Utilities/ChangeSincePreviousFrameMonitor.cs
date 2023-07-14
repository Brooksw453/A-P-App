using System;

public class ChangeSincePreviousFrameMonitor<T>
{
    private readonly Func<T> _getValue;
    private T _previousValue;
    public bool IsValueUpdatedSinceLastUpdateCall { get; private set; }

    public T CurrentValue { get; private set; }

    public ChangeSincePreviousFrameMonitor(Func<T> getValue)
    {
        _getValue = getValue;
    }

    public void Update()
    {
        IsValueUpdatedSinceLastUpdateCall = false;

        CurrentValue = _getValue();
        if (!_previousValue.Equals(CurrentValue))
        {
            _previousValue = CurrentValue;
            IsValueUpdatedSinceLastUpdateCall = true;
        }
    }
}
