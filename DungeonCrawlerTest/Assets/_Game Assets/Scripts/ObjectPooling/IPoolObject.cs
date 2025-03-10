using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    public int GetOwnerPoolIndex();
    public GameObject GetGameObject();
    public void OnReturnedToPool() { }
}
