using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUpFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smooth = 0.1f;

    void LateUpdate()
    {
        transform.up = Vector3.MoveTowards(transform.up, target.up, smooth);
    }
}
