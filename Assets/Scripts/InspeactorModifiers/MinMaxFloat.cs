using UnityEngine;

[System.Serializable]
public class MinMaxFloat
{
    public float min;
    public float max;
    public float RandomValue
    {
        get
        {
            if (min == max)
                return min;
            return Random.Range(min, max);
        }
    }
}

[System.AttributeUsage(System.AttributeTargets.Field)]
public class MinMaxRangeAttribute : PropertyAttribute
{
    public float min;
    public float max;

    public MinMaxRangeAttribute(float aMin, float aMax)
    {
        min = aMin;
        max = aMax;
    }
    public bool IsInRange(float aValue)
    {
        return aValue >= min && aValue <= max;
    }
}