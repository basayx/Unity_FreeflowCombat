using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PoolObjectBase : MonoBehaviour, IPoolObject
{
    [Header("Object Pooling")]
    public int OwnerPoolIndex = -1;
    private void OnValidate()
    {
        // Debug.Log("Validating: " + gameObject.name);
        var objectPoolingInstance = ObjectPoolingManager.Instance;
        if (objectPoolingInstance)
        {
            OwnerPoolIndex = objectPoolingInstance.ObjectPools.FindIndex(x => x.GetPoolObjectPrefab().GetGameObject().TryGetComponent(out PoolObjectBase poolObjectBase) && poolObjectBase == this);
        
            // UnityEditor.EditorUtility.SetDirty(this);
        }
    }

    [ContextMenu(nameof(ManuallyValidate))]
    public void ManuallyValidate()
    {
        OnValidate();
    }

    public int GetOwnerPoolIndex()
    {
        return OwnerPoolIndex;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public virtual void OnReturnedToPool()
    {
    }

    private void OnDestroy()
    {
        if (ObjectPoolingManager.Instance)
            ObjectPoolingManager.Instance.APoolObjectDestroyed(this);
    }
}
