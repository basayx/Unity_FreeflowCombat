using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectOnBecameVisible : MonoBehaviour
{
    public UnityAction OnBecameVisibleAction;
    
    private void OnBecameVisible()
    {
        OnBecameVisibleAction?.Invoke();
    }
}
