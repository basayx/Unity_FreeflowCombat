using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookToCameraAtOnEnable : MonoBehaviour
{
    private bool _isOnEnabled;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private bool alwaysUpdate = false;
    
    private void OnEnable()
    {
        if (!_isOnEnabled || !alwaysUpdate)
            StartCoroutine(LookToCameraCoroutine());
        _isOnEnabled = true;
    }

    IEnumerator LookToCameraCoroutine()
    {
        var isFirstTime = true;
        while (isFirstTime || alwaysUpdate)
        {
            isFirstTime = false;
            
            while (!_cameraTransform)
            {
                if (Camera.main != null) _cameraTransform = Camera.main.transform;
                yield return null;
            }

            transform.forward = _cameraTransform.forward;
            yield return null;
        }
    }

    private void OnDisable()
    {
        _isOnEnabled = false;
        StopAllCoroutines();
    }
}
