using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class EmissionHitEffector : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();

    private bool _isInEffect = false;
    
    [SerializeField] private Color defaultColor = Color.black;
    [SerializeField] private Color effectColor = Color.gray;
    
    [SerializeField] private float _effectDuration = 0.06f;

    [SerializeField] private bool useSharedMaterials;
    
    private void Awake()
    {
        if (renderers.Count <= 0)
            renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());

        if (!useSharedMaterials)
        {
            foreach (var r in renderers)
            {
                if (!r) continue;
                Material[] mats = new Material[r.materials.Length];
                for (var i = 0; i < r.materials.Length; i++)
                {
                    mats[i] = Instantiate(r.materials[i]);
                }
                r.materials = mats;
            }
        }
    }

    public void DoEffect()
    {
        if (_isInEffect)
            return;
        _isInEffect = true;
        
        foreach (var r in renderers)
        {
            if (!r) continue;
            if (useSharedMaterials)
            {
                foreach (var mat in r.sharedMaterials)
                {
                    mat.DOKill();
                    mat.DOColor(effectColor, "_EmissionColor", _effectDuration).SetDelay(_effectDuration).OnComplete(() =>
                    {
                        mat.DOColor(defaultColor, "_EmissionColor", _effectDuration);
                        if (mat == r.materials[^1])
                            _isInEffect = false;
                    });
                }
            }
            else
            {
                foreach (var mat in r.materials)
                {
                    mat.DOKill();
                    var mat1 = mat;
                    mat.DOColor(effectColor, "_EmissionColor", _effectDuration).SetDelay(_effectDuration).OnComplete(() =>
                    {
                        mat1.DOColor(defaultColor, "_EmissionColor", _effectDuration);
                        if (mat1 == r.materials[^1])
                            _isInEffect = false;
                    });
                }
            }
        }
    }
}
