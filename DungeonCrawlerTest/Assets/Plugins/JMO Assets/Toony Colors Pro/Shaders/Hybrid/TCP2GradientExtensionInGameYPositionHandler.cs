using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TCP2GradientExtensionInGameYPositionHandler : MonoBehaviour
{
    [SerializeField] private Transform parent;
    private Renderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (parent && _renderer)
            _renderer.sharedMaterial.SetFloat("_parentY", parent.localPosition.y);
    }
}
