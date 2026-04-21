using UnityEngine;

[System.Serializable]
public class StatResource
{
    public int MaxValue { get; private set; }
    public int CurrentValue { get; private set; }

    public System.Action<int, int> OnValueChanged;

    public StatResource(int maxValue)
    {
        MaxValue = maxValue;
        CurrentValue = maxValue;
    }

    public void Modify(int amount)
    {
        CurrentValue += amount;
        CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
        OnValueChanged?.Invoke(CurrentValue, MaxValue);
    }

    public void SetMaxValue(int newMax, bool refill = true)
    {
        MaxValue = newMax;
        if (refill) CurrentValue = MaxValue;
        else CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);

        OnValueChanged?.Invoke(CurrentValue, MaxValue);
    }
    public void SetCurrentValue(int value)
    {
        CurrentValue = Mathf.Clamp(value, 0, MaxValue);
        OnValueChanged?.Invoke(CurrentValue, MaxValue);
    }
    public bool IsEmpty()
    {
        return CurrentValue <= 0;
    }
}