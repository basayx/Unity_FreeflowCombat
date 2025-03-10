using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    private static ObjectPoolingManager _instance;
    public static ObjectPoolingManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<ObjectPoolingManager>();
            }
            return _instance;
        }
        set => _instance = value;
    }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        ManuallyValidate();
    }

    private void OnValidate()
    {
        ManuallyValidate();
    }

    [ContextMenu(nameof(ManuallyValidate))]
    void ManuallyValidate()
    {
        int poolIndex = 0;
        foreach (ObjectPool objectPool in ObjectPools)
        {
            var prefab = objectPool.GetPoolObjectPrefab();
            if (prefab == null)
            {
                Debug.LogError("Missing prefab at pool index: " + poolIndex);
                continue;
            }
            
            var poolObject = prefab.GetGameObject().GetComponent<PoolObjectBase>();
            if (poolObject) poolObject.ManuallyValidate();
            
            poolIndex++;
        }
    }
    
    public List<ObjectPool> ObjectPools = new List<ObjectPool>();

    [System.Serializable]
    private class DelayedPoolObjectReturn
    {
        public float ReturnTime;
        public IPoolObject PoolObject;
    }
    private List<DelayedPoolObjectReturn> _delayedPoolObjectReturns = new List<DelayedPoolObjectReturn>();

    private void Update()
    {
        foreach (DelayedPoolObjectReturn delayedReturn in _delayedPoolObjectReturns)
        {
            if (delayedReturn.ReturnTime <= Time.time)
            {
                if (delayedReturn.PoolObject != null)
                    ReturnObjectToPool(delayedReturn.PoolObject);
                _delayedPoolObjectReturns.Remove(delayedReturn);
                break;
            }
        }
    }

    public int GetIndexOfAPoolByPrefab(IPoolObject prefab)
    {
        return ObjectPools.FindIndex(pool => pool.GetPoolObjectPrefab().GetGameObject() == prefab.GetGameObject());
    }
    public IPoolObject GetPrefabObjectOfAPool(int poolNo)
    {
        return ObjectPools[poolNo].GetPoolObjectPrefab();
    }
    
    public IPoolObject GetObjectFromPool(int poolNo)
    {
        return GetObjectFromPool(GetPrefabObjectOfAPool(poolNo));
    }
    public IPoolObject GetObjectFromPool(PoolObjectBase poolObjectBase)
    {
        return GetObjectFromPool(GetPrefabObjectOfAPool(poolObjectBase.GetOwnerPoolIndex()));
    }
    public void CreateNewObjectForPool(IPoolObject poolObjectBase)
    {
        ReturnObjectToPool(SpawnPoolObject(GetPrefabObjectOfAPool(poolObjectBase.GetOwnerPoolIndex()).GetGameObject()));
    }

    public IPoolObject GetObjectFromPool(IPoolObject objectPrefab)
    {
        var pool = ObjectPools[objectPrefab.GetOwnerPoolIndex()];
        var obj = pool.GetObject();
        obj.GetGameObject().transform.localScale = objectPrefab.GetGameObject().transform.localScale;
        obj.GetGameObject().transform.rotation = objectPrefab.GetGameObject().transform.rotation;
        obj.GetGameObject().SetActive(true);
        return obj;
    }

    public void ReturnObjectToPool(IPoolObject poolObject)
    {
        if (poolObject == null)
            return;
        var pool = ObjectPools[poolObject.GetOwnerPoolIndex()];
        pool.Objects.Push(poolObject);
        poolObject.OnReturnedToPool();
        poolObject.GetGameObject().SetActive(false);
        poolObject.GetGameObject().transform.SetParent(transform);
    }
    
    public void ReturnObjectToPool(IPoolObject poolObject, float delay)
    {
        _delayedPoolObjectReturns.Add(new DelayedPoolObjectReturn()
        {
            ReturnTime = Time.time + delay,
            PoolObject = poolObject
        });
    }

    public void APoolObjectDestroyed(IPoolObject poolObject)
    {
        var d = _delayedPoolObjectReturns.Find(x => x.PoolObject == poolObject);
        if (d != null) _delayedPoolObjectReturns.Remove(d);
    }

    public IPoolObject SpawnPoolObject(GameObject objPrefab)
    {
        return Instantiate(objPrefab).GetComponent<IPoolObject>();
    }

}

[System.Serializable]
public class ObjectPool
{
    [SerializeField] private GameObject poolObjectPrefab;
    public IPoolObject GetPoolObjectPrefab() { return poolObjectPrefab ? poolObjectPrefab.GetComponent<IPoolObject>() : default; }
    
    public Stack<IPoolObject> Objects = new Stack<IPoolObject>();
    
    public bool IsPoolEmpty()
    {
        return Objects.Count <= 0;
    }

    public IPoolObject GetObject()
    {
        return IsPoolEmpty() ? ObjectPoolingManager.Instance.SpawnPoolObject(poolObjectPrefab) : Objects.Pop();
    }
}
