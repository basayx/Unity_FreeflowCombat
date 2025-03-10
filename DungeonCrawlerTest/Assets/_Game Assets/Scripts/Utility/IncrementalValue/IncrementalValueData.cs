using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IncrementalValueData : ScriptableObject
{
    [Header("Curve")]
    [SerializeField] protected bool isCurveActive = false;
    [SerializeField] protected AnimationCurve valueIncreaseCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public abstract object Calculate(int level, int curveMaxLevel = -1);
}
