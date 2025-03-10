using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smooth = 0.1f;

    private void Start()
    {
        transform.SetParent(null);
    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, smooth);
    }
}
