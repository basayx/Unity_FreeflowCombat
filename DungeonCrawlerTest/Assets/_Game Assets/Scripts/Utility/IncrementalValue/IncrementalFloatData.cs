using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
public class IncrementalFloatData : IncrementalValueData
{
    [Tooltip("This list must have at least one element!")]
    [SerializeField] private List<float> valuesByLevel = new List<float>();
    [SerializeField] private float valueIncreaseAmountPerLevel;

    private float CalculateWithoutCurve(int level)
    {
        float value = valuesByLevel[^1];
        if (level >= valuesByLevel.Count)
        {
            value += valueIncreaseAmountPerLevel * ((level + 1) - valuesByLevel.Count);
        }
        else
            value = valuesByLevel[level];

        return value;
    }

    public float CalculateFloat(int level, int curveMaxLevel = -1)
    {
        if (!isCurveActive || curveMaxLevel <= 0)
            return CalculateWithoutCurve(level);
        return (float)Math.Round(valueIncreaseCurve.Evaluate(level / (float)curveMaxLevel), 2);
    }

    public override object Calculate(int level, int curveMaxLevel = -1)
    {
        return CalculateFloat(level, curveMaxLevel);
    }

    // [SerializeField] private int testLevel = 10;
    // [ContextMenu("Test")]
    // public void Test()
    // {
    //     Debug.Log(Calculate(testLevel, 99));
    // }
}
