using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class IncrementalIntegerData : IncrementalValueData
{
    [Tooltip("This list must have at least one element!")]
    [SerializeField] public List<int> valuesByLevel = new List<int>();
    [SerializeField] private int valueIncreaseAmountPerLevel;

    private int CalculateWithoutCurve(int level)
    {
        int value = valuesByLevel[^1];
        if (level >= valuesByLevel.Count)
        {
            value += valueIncreaseAmountPerLevel * ((level + 1) - valuesByLevel.Count);
        }
        else
            value = valuesByLevel[level];

        return value;
    }

    public int CalculateInt(int level, int curveMaxLevel = -1)
    {
        if (!isCurveActive || curveMaxLevel <= 0)
            return CalculateWithoutCurve(level);
        return Mathf.RoundToInt(valueIncreaseCurve.Evaluate(level / (float)curveMaxLevel));
    }

    public override object Calculate(int level, int curveMaxLevel = -1)
    {
        return CalculateInt(level, curveMaxLevel);
    }
}
